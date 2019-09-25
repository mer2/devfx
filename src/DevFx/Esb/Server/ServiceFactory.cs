using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using DevFx.Esb.Serialize;
using DevFx.Esb.Server.Settings;
using DevFx.Logging;
using DevFx.Core;
using Microsoft.AspNetCore.Http;
using DevFx.Utils;

namespace DevFx.Esb.Server
{
	[Object]
	internal class ServiceFactory : IInitializable<ServiceFactorySetting>, IServiceFactory
	{
		[Autowired]
		protected IObjectService ObjectService { get; set; }
		private ValueProviderFactoryCollection factories;
		private Dictionary<string, ServiceContainer> services;
		private Regex regexRoute;
		protected ServiceFactorySetting Setting { get; set; }
		public void Init(ServiceFactorySetting setting) {
			this.Setting = setting;

			this.factories = new ValueProviderFactoryCollection {
				new CustomValueProviderFactory(),
				new FormValueProviderFactory(),
				new QueryStringValueProviderFactory()
			};

			//添加服务
			this.services = new Dictionary<string, ServiceContainer>(StringComparer.OrdinalIgnoreCase);
			this.AddServicesFromAttributes();

			this.regexRoute = new Regex(this.Setting.RouteUrl + this.Setting.PathRegex, RegexOptions.Compiled | RegexOptions.Singleline);

			ServiceExtenderAttribute.ExtenderInit<IServiceFactory, ServiceFactoryExtenderAttribute>(this.ObjectService, this);
		}

		protected internal virtual void AddServicesFromAttributes() {
			var dict = this.ObjectService.AsObjectServiceInternal().CoreAttributes;
			if(!dict.TryGetValue(typeof(ServiceExportAttribute), out var attributes)) {
				return;
			}
			if(attributes == null || attributes.Count <= 0) {
				return;
			}
			var dictionary = this.services;
			foreach (ServiceExportAttribute item in attributes) {
				if(item.OwnerType == null) {
					continue;
				}
				dictionary.Add(item.Name, new ServiceContainer {
					Name = item.Name,
					AliasName = item.AliasName,
					Authorization = item.Authorization,
					ContractType = item.OwnerType,
					ServiceType = item.ServiceType,
					Inherits = item.Inherits
				});
			}
		}

		protected internal virtual void ResultHandle(ServiceContext ctx) {
			this.OnResponse(ctx);
			if (ctx.Responsed) {
				return;
			}
			var result = ctx.ResultValue;
			var context = ctx.HttpContext;
			var contentType = ctx.HttpContext.Request.ContentType;
			var serializer = SerializerFactory.Current.GetSerializer(contentType) ?? SerializerFactory.Current.Default;
			if (result != null) {
				var isResult = result is IAOPResult;
				if (!isResult) {
					isResult = Array.Exists(result.GetType().GetInterfaces(), t => t.IsGenericType && t.GetGenericTypeDefinition() == typeof(IAOPResult<>));
				}
				if (!isResult) {
					result = AOPResult.Create(0, null, result, null);
				}
			} else {
				result = AOPResult.Success();
			}
			context.Response.ContentType = serializer.ContentType + "; charset=utf-8";
			using (var ms = new MemoryStream()) {
				serializer.Serialize(ms, result, new Hashtable { { "ContentType", contentType } });
				context.Response.ContentLength = ms.Length;
				ms.WriteTo(context.Response.Body);
			}
		}

		protected internal virtual void ResultHandle(ServiceContext ctx, object result) {
			ctx.ResultInitialized = true;
			ctx.ResultValue = result;
			this.ResultHandle(ctx);
		}

		protected internal virtual void OnRequest(ServiceContext ctx) {
			this.Request?.Invoke(ctx);
		}

		protected internal virtual void OnCalling(ServiceContext ctx) {
			this.Calling?.Invoke(ctx);
		}

		protected internal virtual void OnCalled(ServiceContext ctx) {
			this.Called?.Invoke(ctx);
		}

		protected internal virtual void OnResponse(ServiceContext ctx) {
			this.Response?.Invoke(ctx);
		}

		protected internal virtual void OnError(ServiceContext ctx) {
			var e = ctx.Error;
			if (e == null) {
				return;
			}

			//TODO 把请求数据都保存起来以供DEBUG参考

			LogService.Error($"ServiceFactory Error:{Environment.NewLine}{e}", this);
			var errorHandler = this.Error;
			if(errorHandler != null) {
				errorHandler(ctx);
			} else {
				var errorNo = -500;
				var message = e.Message;
				var exception = FindSourceException<ResultException>(e);
				if (exception != null) {
					if (exception.ResultNo != 0) {
						errorNo = exception.ResultNo;
					}
					message = exception.Message;
				}
				this.ResultHandle(ctx, AOPResult.Create(errorNo, message));
				ctx.Error = null;
			}
		}

		private static T FindSourceException<T>(Exception e) where T : Exception {
			var expectedExceptionType = typeof (T);
			while(e != null) {
				if(expectedExceptionType.IsInstanceOfType(e)) {
					return (T)e;
				}
				e = e.InnerException;
			}
			return null;
		}

		public bool IsHandleable(HttpContext context) {//是否是Esb请求
			var path = context?.Request.Path;
			if (path == null || !path.Value.HasValue) {
				return false;
			}
			var matched = path.Value.Value.StartsWith(this.Setting.RouteUrl, StringComparison.OrdinalIgnoreCase);
			return matched;
		}

		internal static readonly AsyncLocal<ServiceContext> ServiceContextCurrent = new AsyncLocal<ServiceContext>();
		public void ProcessRequest(HttpContext context) {
			var serviceContext = new ServiceContext {
				HttpContext = context
			};
			ServiceContextCurrent.Value = serviceContext;
			try {
				this.ProcessRequestInternal(serviceContext);
			} catch(Exception e) {
				serviceContext.Error = e;
				this.OnError(serviceContext);
				if(serviceContext.Error != null) {
					throw serviceContext.Error;
				}
			}
		}

		protected virtual IAOPResult<ServiceHandler> GetServiceHandler(ServiceContext ctx) {
			var httpContext = ctx.HttpContext;
			if (httpContext.Request.Path.HasValue) {
				var path = httpContext.Request.Path.Value;
				var match = this.regexRoute.Match(path);
				ctx.ServiceName = match.Groups["serviceName"]?.Value;
				ctx.MethodName = match.Groups["methodName"]?.Value;
			}
			var serviceName = ctx.ServiceName;
			if(string.IsNullOrEmpty(serviceName)) {
				return AOPResult.Failed<ServiceHandler>(-500, "service missing");
			}
			if(!this.services.ContainsKey(serviceName)) {
				return AOPResult.Failed<ServiceHandler>(-500, "service not found:" + serviceName);
			}
			var container = this.services[serviceName];
			ctx.ServiceContainer = container;
			if(container.ServiceHandler == null) {
				lock(container) {
					if(container.ServiceHandler == null) {
						container.ServiceHandler = this.CreateHandler(container);
					}
				}
			}
			return AOPResult.Success(container.ServiceHandler);
		}

		protected virtual void ProcessRequestInternal(ServiceContext ctx) {
			var result = this.GetServiceHandler(ctx);
			if (result.ResultNo != 0) {
				this.ResultHandle(ctx, AOPResult.Failed(result.ResultNo, result.ResultDescription));
				return;
			}
			var serviceHandler = result.ResultAttachObject;
			if(serviceHandler == null) {
				this.ResultHandle(ctx, AOPResult.Failed(-500, "serviceHandler not found"));
				return;
			}
			ctx.ValueProvider = this.factories.GetValueProvider(ctx.HttpContext);
			serviceHandler.ProcessRequest(ctx);
		}

		protected virtual ServiceHandler CreateHandler(ServiceContainer container) {
			var contractType = container.ContractType;
			if(contractType == null || !contractType.IsInterface) {
				throw new ArgumentException("ContractType cannot be created.");
			}
			return new ServiceHandler(this.ObjectService, container, contractType, this);
		}

		protected virtual IDictionary<string, IServiceContainer> GetServiceContainers() {
			return this.services.ToDictionary(k => k.Key, v => (IServiceContainer)v.Value);
		}

		#region Events

		protected virtual event Action<ServiceContext> Request;
		protected virtual event Action<ServiceContext> Calling;
		protected virtual event Action<ServiceContext> Called;
		protected virtual event Action<ServiceContext> Response;
		protected virtual event Action<ServiceContext> Error;

		#endregion

		#region IServiceFactory Members

		IDictionary<string, IServiceContainer> IServiceFactory.GetServiceContainers() {
			return this.GetServiceContainers();
		}

		event Action<ServiceContext> IServiceFactory.Request {
			add => this.Request += value;
			remove => this.Request -= value;
		}

		event Action<ServiceContext> IServiceFactory.Calling {
			add => this.Calling += value;
			remove => this.Calling -= value;
		}

		event Action<ServiceContext> IServiceFactory.Called {
			add => this.Called += value;
			remove => this.Called -= value;
		}

		event Action<ServiceContext> IServiceFactory.Response {
			add => this.Response += value;
			remove => this.Response -= value;
		}

		event Action<ServiceContext> IServiceFactory.Error {
			add => this.Error += value;
			remove => this.Error -= value;
		}

		#endregion
	}
}

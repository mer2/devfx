using System;
using System.Linq;
using System.Collections.Generic;
using System.Reflection;
using DevFx.Core;
using DevFx.Utils;

namespace DevFx.Esb.Server
{
	internal class ServiceHandler
	{
		public ServiceHandler(IObjectService objectService, ServiceContainer container, Type contractType, ServiceFactory serviceFactory) {
			this.ObjectService = objectService;
			this.Container = container;
			this.ServiceFactory = serviceFactory;
			this.InitService(contractType);
		}
		protected IDictionary<string, MethodInfo> Methods { get; private set; }//方法别名
		protected IDictionary<string, MethodInfo> FullNameMethods { get; private set; }//完整名称的方法
		protected ServiceFactory ServiceFactory { get; }
		protected ServiceContainer Container { get; }
		protected IObjectService ObjectService { get; }

		protected void InitService(Type contractType) {
			var dict = this.Methods = new Dictionary<string, MethodInfo>(StringComparer.OrdinalIgnoreCase);
			var allMethods = this.FullNameMethods = new Dictionary<string, MethodInfo>();
			InitMethods(contractType, dict, allMethods);
			if (this.Container.Inherits) {
				var types = contractType.GetInterfaces();
				if (types.Length > 0) {
					foreach (var type in types) {
						InitMethods(type, dict, allMethods);
					}
				}
			}
		}

		private static void InitMethods(Type contractType, IDictionary<string, MethodInfo> dict, IDictionary<string, MethodInfo> allMethods) {
			var methods = contractType.GetMethods();
			foreach(var method in methods) {
				var mn = method.ToString();
				if (allMethods.ContainsKey(mn)) {
					allMethods.Remove(mn);
				}
				allMethods.Add(mn, method);
				var name = method.Name;
				if(method.IsDefined(typeof(MethodNameAttribute), true)) {
					var methodName = method.GetCustomAttribute<MethodNameAttribute>(true);
					if(methodName != null) {
						name = methodName.Name;
					}
				}
				if(dict.ContainsKey(name)) {
					dict.Remove(name);
				}
				dict.Add(name, method);
			}
		}

		protected virtual void ResultHandle(ServiceContext ctx) {
			this.ServiceFactory.ResultHandle(ctx);
		}

		protected virtual void ResultHandle(ServiceContext ctx, object result) {
			this.ServiceFactory.ResultHandle(ctx, result);
		}

		protected virtual object ConvertTo(ValueProviderResult vpResult, Type type) {
			if(vpResult == null) {
				vpResult = new ValueProviderResult(null, null);
			}
			return vpResult.ConvertTo(type);
		}

		public void ProcessRequest(ServiceContext ctx) {
			var httpContext = ctx.HttpContext;
			var methodName = ctx.MethodName;//优先获取Url里的方法名
			if(string.IsNullOrEmpty(methodName)) {//尝试从请求header里获取方法名
				methodName = httpContext.Request.Headers["$action"];
			}
			var valueProvider = ctx.ValueProvider;
			if(string.IsNullOrEmpty(methodName)) {//尝试从请求体里获取方法名
				var vpResult = valueProvider.GetValue("$action");
				methodName = vpResult?.ConvertTo<string>();
			}
			//保存到字典里供外部使用
			ctx.MethodName = methodName;

			this.ServiceFactory.OnRequest(ctx);
			if(ctx.ResultInitialized) {
				this.ResultHandle(ctx);
				return;
			}

			methodName = ctx.MethodName;
			var method = ctx.CallMethod;
			if(method == null) {
				if(string.IsNullOrEmpty(methodName)) {
					this.ResultHandle(ctx, AOPResult.Create(-500, "method missing"));
					return;
				}
				if(!this.Methods.ContainsKey(methodName)) {
					var vpResult = valueProvider.GetValue("$method");
					var methodFullName = vpResult?.ConvertTo<string>();
					if(!string.IsNullOrEmpty(methodFullName) && this.FullNameMethods.ContainsKey(methodFullName)) {
						method = this.FullNameMethods[methodFullName];
					}
				} else {
					method = this.Methods[methodName];
				}
				ctx.CallMethod = method;
			}
			if(method == null) {
				this.ResultHandle(ctx, AOPResult.Create(-500, "method not found:" + methodName));
				return;
			}

			var container = (ServiceContainer)ctx.ServiceContainer;
			var serviceInstance = ctx.ObjectInstance;
			if(serviceInstance == null) {
				var serviceType = container.ServiceType;
				ctx.ObjectInstance = serviceInstance = serviceType != null ? this.ObjectService.GetOrCreateObject(serviceType) : this.ObjectService.GetObject(container.ContractType);
			}
			if(serviceInstance == null || !container.ContractType.IsInstanceOfType(serviceInstance)) {
				this.ResultHandle(ctx, AOPResult.Create(-500, "serviceInstance create error"));
				return;
			}
			var parameterValues = ctx.CallArguments;
			if(parameterValues == null) {
				var parameters = method.GetParameters();
				ctx.CallArguments = parameterValues = parameters.Select(parameter => this.ConvertTo(valueProvider.GetValue(parameter.Name), parameter.ParameterType)).ToArray();
			}
			this.ServiceFactory.OnCalling(ctx);
			if(!ctx.ResultInitialized) {
				TypeHelper.TryInvoke(serviceInstance, method, out var returnValue, true, parameterValues);
				ctx.ResultInitialized = true;
				ctx.ResultValue = returnValue;
			}
			this.ServiceFactory.OnCalled(ctx);
			this.ResultHandle(ctx);
		}
	}
}

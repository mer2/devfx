using DevFx.Esb.Serialize;
using DevFx.Reflection;
using DevFx.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Text;

namespace DevFx.Esb.Client
{
	[Object]
	public class HttpRealProxy : IHttpRealProxy, IServiceDispatchProxy
	{
		public HttpRealProxy(Type proxyType, string url, string contentType) {
			this.ProxyType = proxyType;
			this.ProxyUrl = url;
			if (!string.IsNullOrEmpty(contentType)) {
				this.ContentType = contentType;
			}
		}

		private IHttpRealProxyFactoryInternal factory;
		public virtual string ContentType { get; set; } = "application/json";

		protected virtual object Invoke(MethodInfo targetMethod, object[] args) {
			var parameters = this.BuildParameters(targetMethod, args);
			var returnValue = this.Call(parameters, targetMethod);
			return returnValue;
		}
		object IServiceDispatchProxy.Invoke(MethodInfo targetMethod, object[] args) {
			return this.Invoke(targetMethod, args);
		}

		protected virtual IDictionary<string, object> BuildParameters(MethodInfo targetMethod, object[] args) {
			var parameters = new Dictionary<string, object>();
			var ps = targetMethod.GetParameters();
			for(var i = 0; i < ps.Length; i++) {
				parameters.Add(ps[i].Name, args[i]);
			}
			return parameters;
		}

		protected virtual string GetRealUrl(string proxyUrl) {
			var realUrl = this.Factory.GetRealProxyUrl(proxyUrl);
			if(!string.IsNullOrEmpty(realUrl)) {
				return realUrl;
			}
			return proxyUrl;
		}

		protected virtual HttpClient GetHttpClient() {
			return HttpClientHelper.GetHttpClientFactory().CreateClient(this.GetHashCode().ToString());
		}

		protected virtual ProxyContext PrepareProxyContext(IDictionary<string, object> parameters, MethodInfo targetMethod, string contentType) {
			var serializer = SerializerFactory.Current.GetSerializer(contentType) ?? SerializerFactory.Current.Default;
			var ctx = new ProxyContext {
				ProxyInstance = this,
				Serializer = serializer,
				CallMethod = targetMethod,
				Parameters = parameters,
				ProxyUrl = this.GetRealUrl(this.ProxyUrl),
			};
			return ctx;
		}

		protected virtual object Call(IDictionary<string, object> parameters, MethodInfo targetMethod) {
			var ctx = this.PrepareProxyContext(parameters, targetMethod, this.ContentType);
			this.Factory?.OnCalling(ctx);
			if(ctx.ResultInitialized) {
				return ctx.ResultValue;
			}
			var url = ctx.ProxyUrl;
			if(string.IsNullOrEmpty(url)) {
				throw new ResultException(-1, $"远程服务地址未找到：{this.ProxyType.FullName}");
			}
			if(!url.StartsWith("http://", true, null) && !url.StartsWith("https://", true, null)) {
				throw new ResultException(-1, $"远程服务地址仅支持http和https：{this.ProxyType.FullName}，当前地址为：{url}");
			}
			var request = new HttpRequestMessage(HttpMethod.Post, url);
			ctx.HttpRequest = request;
			this.Factory?.OnRequest(ctx);
			this.PrepareRequest(ctx);
			if (ctx.HttpRequest != null) {
				request = ctx.HttpRequest;
			}
			object returnValue;
			using(var httpClient = this.GetHttpClient()) {
				var response = httpClient.SendAsync(request).GetAwaiter().GetResult();
				if (response.StatusCode == HttpStatusCode.OK) {
					returnValue = this.ResponseHandle(ctx, response);
				} else {
					throw new ResultException(-1, $"Server Error: {(int)response.StatusCode}/{response.ReasonPhrase}");
				}
			}
			this.Factory?.OnCalled(ctx);
			if(ctx.ResultInitialized) {
				returnValue = ctx.ResultValue;
			}
			return returnValue;
		}

		protected virtual void PrepareRequest(ProxyContext ctx) {
			var parameters = ctx.Parameters;
			var serializer = ctx.Serializer;
			var request = ctx.HttpRequest;
			var methodInfo =ctx.CallMethod;
			parameters.Add("$method", methodInfo.ToString());
			var methodName = this.GetCallMethodName(ctx);
			parameters.Add("$action", methodName);
			request.Headers.Add("$action", methodName);
			ctx.ProxyUrl += "/" + methodName;
			using(var ms = new MemoryStream()) {
				serializer.Serialize(ms, parameters, null);
				ms.Position = 0;
				using (var sr = new StreamReader(ms)) {
					request.Content = new StringContent(sr.ReadToEnd(), Encoding.UTF8, this.ContentType);
				}
			}
		}

		protected virtual string GetCallMethodName(ProxyContext ctx) {
			return ctx.CallMethod.Name;
		}

		//获取方法期望的返回值，如果期望返回的不是IAOPResult，则包装成此类型
		protected virtual Type GetExpectedReturnType(ProxyContext ctx) {
			var type = ctx.ExpectedReturnType;
			if (type == null) {
				var methodInfo = ctx.CallMethod;
				var returnType = methodInfo.ReturnType;
				if (returnType.IsInterface) { //方法定义的返回值是接口，需要转换成实现
					if (returnType.IsGenericType && returnType.GetGenericTypeDefinition() == typeof(IAOPResult<>)) {
						//返回值是IAOPResult<>（接口），则返回默认实现
						var esbType = typeof(AOPResult<>);
						type = esbType.MakeGenericType(returnType.GetGenericArguments());
					} else {
						type = typeof(AOPResult);
					}
				} else if(returnType == typeof(void))  {
					type = typeof(AOPResult);
				} else { //方法定义的返回值是实际类型，则需要判断是否为IAOPResult的实现
					if (typeof(IAOPResult).IsAssignableFrom(returnType)) {
						type = returnType;
					} else {
						//不是IAOPResult的实现，则包装
						var esbType = typeof(AOPResult<>);
						type = esbType.MakeGenericType(returnType);
					}
				}
			}
			return type;
		}

		protected virtual object ResponseHandle(ProxyContext ctx, HttpResponseMessage response) {
			var serializer = ctx.Serializer;
			var expectedReturnType = this.GetExpectedReturnType(ctx);//返回的是IAOPResult的实现
			var stream = response.Content.ReadAsStreamAsync().GetAwaiter().GetResult();
			if (stream == null) {
				throw new ResultException(-1, "Server No Response");
			}
			var aop = (IAOPResult)serializer.Deserialize(stream, expectedReturnType, null);
			if(aop == null) {
				throw new ResultException(-2, "Unexpected Response");
			}

			this.Factory?.OnResultHandling(ctx, aop);
			if(ctx.ResultInitialized) {
				return ctx.ResultValue;
			}
			var methodInfo = ctx.CallMethod;
			var returnType = methodInfo.ReturnType;
			return this.ResultHandle(aop, returnType, serializer);
		}

		protected virtual object ResultHandle(IAOPResult aop, Type returnType, ISerializer serializer) {
#if DEBUG
			Logging.LogService.Debug(aop.ToString(), this);
#endif
			object returnValue = null;
			if (!typeof(IAOPResult).IsAssignableFrom(returnType)) {
				//不是返回IAOPResult，需要去掉IAOPResult
				if (aop.ResultNo == 0) {
					if (returnType != typeof(void)) {//不是无返回值
						if (aop.ResultAttachObject != null) {
							returnValue = returnType.IsInstanceOfType(aop.ResultAttachObject) ? aop.ResultAttachObject : serializer.Convert(aop.ResultAttachObject, returnType, null);
						}
					}
				} else {
					throw new ResultException(aop.ResultNo, aop.ResultDescription);
				}
			} else {
				returnValue = aop;
			}
			return returnValue;
		}

		public virtual void Init(IHttpRealProxyFactoryInternal factory = null, IDictionary options = null) {
			this.Factory = factory;
		}

		//获取实际的代理实例
		public object GetTransparentProxy() {
			if (this.proxyInstance == null) {
				lock (this) {
					if (this.proxyInstance == null) {
						var instance = TypeHelper.CreateServiceDispatchProxy(this.ProxyType, this);
						this.proxyInstance = instance;
					}
				}
			}
			return this.proxyInstance;
		}
		private object proxyInstance;

		public virtual IHttpRealProxyFactoryInternal Factory {
			get => this.factory ?? (this.factory = ObjectService.GetObject<IHttpRealProxyFactoryInternal>());
			private set => this.factory = value;
		}

		public virtual Type ProxyType { get; }
		public virtual string ProxyUrl { get; }
	}
}

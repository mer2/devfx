using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DevFx.Core;
using DevFx.Esb.Client.Settings;

namespace DevFx.Esb.Client
{
	[Object]
	internal class HttpRealProxyFactory : IInitializable<HttpRealProxyFactorySetting>, IHttpRealProxyFactory, IHttpRealProxyFactoryInternal
	{
		[Autowired]
		protected IObjectService ObjectService { get; set; }
		private readonly IDictionary<string, IHttpRealProxy> proxies = new Dictionary<string, IHttpRealProxy>();
		private readonly IDictionary<string, string> urlMappings = new Dictionary<string, string>();
		private readonly List<HttpRealProxyAttribute> handlers = new List<HttpRealProxyAttribute>();

		void IInitializable<HttpRealProxyFactorySetting>.Init(HttpRealProxyFactorySetting setting) {
			//Url映射
			var urls = setting?.UrlMapping;
			if(urls != null && urls.Length > 0) {
				foreach(var urlMapping in urls) {
					this.urlMappings.Add(urlMapping.Name, urlMapping.Url);
				}
			}

			var coreAttributes = this.ObjectService.AsObjectServiceInternal().CoreAttributes;
			//初始化处理Url的处理器，比如可以处理形如下面格式的Url：esb://
			coreAttributes.TryGetValue(typeof(HttpRealProxyAttribute), out var list);
			if(list != null && list.Count > 0) {
				foreach(HttpRealProxyAttribute attribute in list) {
					this.handlers.Add(attribute);
				}
			}
			//初始化扩展器
			ServiceExtenderAttribute.ExtenderInit<IHttpRealProxyFactory, HttpRealProxyFactoryExtenderAttribute>(this.ObjectService, this);
		}

		public string GetRealProxyUrl(string url) {
			string realUrl = null;
			if (!string.IsNullOrEmpty(url)) {
				this.urlMappings.TryGetValue(url, out realUrl);
			}
			return realUrl;
		}

		public object GetProxyObject(Type objectType, string url, string contentType, IDictionary options = null) {
			var proxy = this.GetHttpRealProxy(objectType, url, contentType, options);
			return proxy?.GetTransparentProxy();
		}

		public T GetProxyObject<T>(string url, string contentType, IDictionary options = null) where T : class {
			var instance = this.GetProxyObject(typeof(T), url, contentType, options);
			return (T)instance;
		}

		public IHttpRealProxy GetHttpRealProxy(Type objectType, string url, string contentType, IDictionary options = null) {
			if(objectType == null || string.IsNullOrEmpty(url)) {
				return null;
			}
			var key = $"{objectType.AssemblyQualifiedName}_{url}";
			IHttpRealProxy proxy;
			lock(this.proxies) {
				if(!this.proxies.TryGetValue(key, out proxy)) {
					proxy = this.CreateHttpRealProxy(objectType, url, contentType, options);
					this.proxies.Add(key, proxy);
				}
			}
			return proxy;
		}

		protected IHttpRealProxy CreateHttpRealProxy(Type objectType, string url, string contentType, IDictionary options = null) {
			if (contentType == null) {
				contentType = string.Empty;
			}
			IHttpRealProxy proxy = null;
			//根据Url前缀选择相应的代理，比如前缀esb://
			var attribute = this.handlers.FirstOrDefault(x => url.StartsWith(x.Name, StringComparison.InvariantCultureIgnoreCase));
			if (attribute != null) {
				proxy = this.ObjectService.CreateObject(attribute.OwnerType, objectType, url, contentType) as IHttpRealProxy;
			}
			//未找到，则使用缺省的
			if (proxy == null) {
				proxy = this.ObjectService.CreateObject<IHttpRealProxy>(objectType, url, contentType);
			}
			proxy?.Init(this, options);
			return proxy;
		}

		public event Action<ProxyContext> Calling;
		public event Action<ProxyContext> Request;
		public event Action<ProxyContext, IAOPResult> ResultHandling;
		public event Action<ProxyContext> Called;

		public void OnCalling(ProxyContext ctx) {
			this.Calling?.Invoke(ctx);
		}
		public void OnRequest(ProxyContext ctx) {
			this.Request?.Invoke(ctx);
		}
		public void OnResultHandling(ProxyContext ctx, IAOPResult result) {
			this.ResultHandling?.Invoke(ctx, result);
		}
		public void OnCalled(ProxyContext ctx) {
			this.Called?.Invoke(ctx);
		}
	}
}

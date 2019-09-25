using System;
using System.Collections.Generic;
using System.Reflection;

namespace DevFx.Esb.Client
{
	public static class HttpProxyHelper
	{
		private static readonly IDictionary<string, object> callers = new Dictionary<string, object>();
		public static T GetHttpCaller<T>(string url, string contentType, Func<Type, string, string, HttpRealProxy> creator = null) where T : class {
			if (string.IsNullOrEmpty(url)) {
				return default;
			}
			object instance;
			lock (callers) {
				if(!callers.TryGetValue(url, out instance)) {
					lock(callers) {
						if(!callers.TryGetValue(url, out instance)) {
							var type = typeof(T);
							var proxy = creator != null ? creator(type, url, contentType) : new HttpProxy(typeof(T), url, contentType);
							instance = proxy.GetTransparentProxy();
							callers.Add(url, instance);
						}
					}
				}
			}
			return (T)instance;
		}

		private class HttpProxy : HttpRealProxy
		{
			public HttpProxy(Type proxyType, string url, string contentType) : base(proxyType, url, contentType) {
			}

			protected override ProxyContext PrepareProxyContext(IDictionary<string, object> parameters, MethodInfo methodMessage, string contentType) {
				var ctx = base.PrepareProxyContext(parameters, methodMessage, contentType);
				ctx.ProxyUrl = this.ProxyUrl;
				return ctx;
			}
		}
	}
}

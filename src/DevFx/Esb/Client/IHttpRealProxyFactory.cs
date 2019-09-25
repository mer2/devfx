using System;
using System.Collections;

namespace DevFx.Esb.Client
{
	[Service]
	public interface IHttpRealProxyFactory
	{
		object GetProxyObject(Type objectType, string url, string contentType, IDictionary options = null);
		T GetProxyObject<T>(string url, string contentType, IDictionary options = null) where T : class;
		IHttpRealProxy GetHttpRealProxy(Type objectType, string url, string contentType, IDictionary options = null);

		event Action<ProxyContext> Calling;
		event Action<ProxyContext> Request;
		event Action<ProxyContext, IAOPResult> ResultHandling;
		event Action<ProxyContext> Called;
	}
}
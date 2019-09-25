using System;
using System.Collections;

namespace DevFx.Esb.Client
{
	[Service]
	public interface IHttpRealProxy
	{
		void Init(IHttpRealProxyFactoryInternal factory = null, IDictionary options = null);
		Type ProxyType { get; }
		string ProxyUrl { get; }
		string ContentType { get; set; }
		object GetTransparentProxy();
	}
}
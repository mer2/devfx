namespace DevFx.Esb.Client
{
	[Service]
	public interface IHttpRealProxyFactoryInternal
	{
		string GetRealProxyUrl(string url);
		void OnCalling(ProxyContext ctx);
		void OnRequest(ProxyContext ctx);
		void OnResultHandling(ProxyContext ctx, IAOPResult result);
		void OnCalled(ProxyContext ctx);
	}
}

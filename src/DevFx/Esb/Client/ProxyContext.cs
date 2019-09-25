using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using DevFx.Esb.Serialize;
using DevFx.Reflection;

namespace DevFx.Esb.Client
{
	public class ProxyContext : CallContext
	{
		public IHttpRealProxy ProxyInstance { get; set; }
		public ISerializer Serializer { get; set; }
		public IDictionary<string, object> Parameters { get; set; }
		public HttpRequestMessage HttpRequest { get; set; }
		public string ProxyUrl { get; set; }//被代理的服务地址，可能是一个别名
		public Type ExpectedReturnType { get; set; }//期待返回值的类型
	}
}
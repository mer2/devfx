using DevFx.Reflection;
using Microsoft.AspNetCore.Http;

namespace DevFx.Esb.Server
{
	public class ServiceContext : CallContext
	{
		public string ServiceName { get; set; }
		public string MethodName { get; set; }
		public HttpContext HttpContext { get; set; }
		public IValueProvider ValueProvider { get; set; }
		public bool Responsed { get; set; }
		public IServiceContainer ServiceContainer { get; set; }

		public static ServiceContext Current => ServiceFactory.ServiceContextCurrent.Value;
	}
}
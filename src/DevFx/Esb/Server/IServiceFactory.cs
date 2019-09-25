using System;
using System.Collections.Generic;

namespace DevFx.Esb.Server
{
	[Service]
	public interface IServiceFactory : IHttpHandler
	{
		IDictionary<string, IServiceContainer> GetServiceContainers();
		event Action<ServiceContext> Request;
		event Action<ServiceContext> Calling;
		event Action<ServiceContext> Called;
		event Action<ServiceContext> Response;
		event Action<ServiceContext> Error;
	}
}
using Microsoft.AspNetCore.Http;

namespace DevFx.Esb.Server
{
    public interface IHttpHandler
    {
		bool IsHandleable(HttpContext context);
		void ProcessRequest(HttpContext context);
	}
}
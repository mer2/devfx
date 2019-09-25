using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace DevFx.Web
{
	[Middleware]
    public class HttpContextHolder : IMiddleware
    {
		public static HttpContext Current => HttpContextCurrent.Value;

	    internal static readonly AsyncLocal<HttpContext> HttpContextCurrent = new AsyncLocal<HttpContext>();
	    async Task IMiddleware.InvokeAsync(HttpContext context, RequestDelegate next) {
		    HttpContextCurrent.Value = context;
		    await next.Invoke(context);
		    HttpContextCurrent.Value = null;
	    }
    }
}

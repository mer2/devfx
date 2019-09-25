using Microsoft.AspNetCore.Http;

namespace DevFx.Esb.Server
{
	internal abstract class ValueProviderFactory
	{
		public const string KeyOfAllFiles = "all_files";
		public const string KeyOfAllValues = "all_values";

		public abstract IValueProvider GetValueProvider(HttpContext httpContext);
	}
}

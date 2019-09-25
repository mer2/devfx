using System.Collections.ObjectModel;
using System.Linq;
using Microsoft.AspNetCore.Http;

namespace DevFx.Esb.Server
{
	internal class ValueProviderFactoryCollection : Collection<ValueProviderFactory>
	{
		public IValueProvider GetValueProvider(HttpContext httpContext) {
			var valueProviders = from factory in this
								 let valueProvider = factory.GetValueProvider(httpContext)
								 where valueProvider != null
								 select valueProvider;
			return new ValueProviderCollection(valueProviders.ToList());
		}
	}
}

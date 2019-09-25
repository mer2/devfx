using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;

namespace DevFx.Esb.Server
{
	internal class QueryStringValueProviderFactory : ValueProviderFactory
	{
		public override IValueProvider GetValueProvider(HttpContext httpContext) {
			if(httpContext == null) {
				throw new ArgumentNullException(nameof(httpContext));
			}
			var query = httpContext.Request.Query;
			if (query == null || query.Count <= 0) {
				return null;
			}
			var dict = new Dictionary<string, object>();
			foreach(var key in query.Keys) {
				dict.Add(key, query[key].ToString());
			}
			if(!dict.ContainsKey(KeyOfAllValues)) {
				dict.Add(KeyOfAllValues, dict);
			}
			return new DictionaryValueProvider(dict, null);
		}
	}
}

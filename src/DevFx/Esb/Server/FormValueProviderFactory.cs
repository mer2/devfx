using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;

namespace DevFx.Esb.Server
{
	internal class FormValueProviderFactory : ValueProviderFactory
	{
		public override IValueProvider GetValueProvider(HttpContext httpContext) {
			if(httpContext == null) {
				throw new ArgumentNullException(nameof(httpContext));
			}
			if (!httpContext.Request.HasFormContentType) {
				return null;
			}
			var forms = new Dictionary<string, object>();
			foreach(var key in httpContext.Request.Form.Keys) {
				forms.Add(key, httpContext.Request.Form[key].ToString());
			}
			var files = httpContext.Request.Form.Files;
			if (files.Count > 0) {
				forms.Add(KeyOfAllFiles, files);
			}
			if (forms.Count > 0) {
				if (!forms.ContainsKey(KeyOfAllValues)) {
					forms.Add(KeyOfAllValues, forms);
				}
			}
			return forms.Count > 0 ? new DictionaryValueProvider(forms, null) : null;
		}
	}
}

using System;
using System.Collections.Generic;
using DevFx.Esb.Serialize;
using Microsoft.AspNetCore.Http;

namespace DevFx.Esb.Server
{
	internal class CustomValueProviderFactory : ValueProviderFactory
	{
		public override IValueProvider GetValueProvider(HttpContext httpContext) {
			if(httpContext == null) {
				throw new ArgumentNullException(nameof(httpContext));
			}

			var data = GetDeserializedObject(httpContext, out var serializer);
			if(data == null) {
				return null;
			}
			var values = data as IDictionary<string, object>;
			if (values != null) {
				if(!values.ContainsKey(KeyOfAllValues)) {
					values.Add(KeyOfAllValues, values);
				}
			}
			return values == null ? null : new DictionaryValueProvider(values, serializer);
		}

		private static object GetDeserializedObject(HttpContext httpContext, out ISerializer serializer) {
			serializer = SerializerFactory.Current.GetSerializer(httpContext.Request.ContentType);

			var data = serializer?.Deserialize(httpContext.Request.Body, typeof(Dictionary<string, object>), null);
			return data;
		}
	}
}

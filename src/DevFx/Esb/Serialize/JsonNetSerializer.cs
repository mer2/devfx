using System;
using System.Collections;
using System.IO;
using DevFx.Utils;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;

namespace DevFx.Esb.Serialize
{
	[Object(Name = nameof(JsonNetSerializer))]
	public class JsonNetSerializer : SerializerBase
	{
		private JsonSerializer serializer;
		protected internal virtual JsonSerializer GetSerializer(IDictionary options = null) {
			return this.serializer ?? (this.serializer = JsonSerializer.Create(this.GetJsonSerializerSettings(options)));
		}

		protected virtual JsonSerializerSettings GetJsonSerializerSettings(IDictionary options = null) {
			return new JsonSerializerSettings {
				Converters = {
					new IsoDateTimeConverter { DateTimeFormat = "yyyy-MM-dd HH:mm:ss.fff" },
					new AOPResultConverter()
				}
			};
		}

		protected override void SerializeInternal(Stream stream, object instance, IDictionary options) {
			var sw = new StreamWriter(stream);
			this.GetSerializer(options).Serialize(new JsonTextWriter(sw), instance);
			sw.Flush();
		}

		protected override object DeserializeInternal(Stream stream, Type expectedType, IDictionary options) {
			var sr = new StreamReader(stream);
			return this.GetSerializer(options).Deserialize(new JsonTextReader(sr), expectedType);
		}

		protected override object ConvertInternal(object instance, Type expectedType, IDictionary options) {
			if(instance is JToken token) {
				return token.ToObject(expectedType, this.serializer);
			}
			return Convert.ChangeType(instance, expectedType);
		}
	}

	public class AOPResultConverter : CustomCreationConverter<IAOPResult>
	{
		public override IAOPResult Create(Type objectType) {//要求返回的是IAOPResult实现
			IAOPResult result;
			if(objectType.IsInterface) { //如果是接口，则返回实现
				if(objectType.IsGenericType) { //是泛型
					var aopType = typeof(AOPResult<>);
					var aopReal = aopType.MakeGenericType(objectType.GenericTypeArguments);
					result = (IAOPResult)TypeHelper.CreateObject(aopReal, typeof(IAOPResult), true);
				} else {
					result = new AOPResult();
				}
			} else {
				result = (IAOPResult)TypeHelper.CreateObject(objectType, typeof(IAOPResult), true);
			}
			return result;
		}
	}
}
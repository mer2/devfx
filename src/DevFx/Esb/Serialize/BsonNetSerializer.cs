using System;
using System.Collections;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Bson;

namespace DevFx.Esb.Serialize
{
	[Object(Name = nameof(BsonNetSerializer))]
	public class BsonNetSerializer : JsonNetSerializer
	{
		protected override JsonSerializerSettings GetJsonSerializerSettings(IDictionary options = null) {
			var settings = base.GetJsonSerializerSettings(options);
			settings.DateFormatHandling = DateFormatHandling.IsoDateFormat;
			return settings;
		}

		protected override void SerializeInternal(Stream stream, object instance, IDictionary options) {
			var sw = new BsonWriter(stream);
			this.GetSerializer(options).Serialize(sw, instance);
			sw.Flush();
		}

		protected override object DeserializeInternal(Stream stream, Type expectedType, IDictionary options) {
			var sr = new BsonReader(stream);
			return this.GetSerializer(options).Deserialize(sr, expectedType);
		}
	}
}
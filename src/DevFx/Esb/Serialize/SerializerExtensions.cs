using System;
using System.Collections;
using System.IO;

namespace DevFx.Esb.Serialize
{
	public static class SerializerExtensions
	{
		public static byte[] Serialize(this ISerializer serializer, object instance, IDictionary options) {
			if(serializer == null || instance == null) {
				return null;
			}
			using(var ms = new MemoryStream()) {
				serializer.Serialize(ms, instance, options);
				return ms.ToArray();
			}
		}

		public static object Deserialize(this ISerializer serializer, byte[] bytes, Type expectedType, IDictionary options) {
			if(serializer == null || bytes == null || bytes.Length <= 0) {
				return null;
			}
			using(var ms = new MemoryStream(bytes)) {
				return serializer.Deserialize(ms, expectedType, options);
			}
		}

		public static T Deserialize<T>(this ISerializer serializer, byte[] bytes, IDictionary options) {
			if(serializer == null || bytes == null || bytes.Length <= 0) {
				return default;
			}
			using(var ms = new MemoryStream(bytes)) {
				return (T)serializer.Deserialize(ms, typeof(T), options);
			}
		}
	}
}

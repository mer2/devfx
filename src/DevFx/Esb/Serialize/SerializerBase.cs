using System;
using System.Collections;
using System.IO;

namespace DevFx.Esb.Serialize
{
	public abstract class SerializerBase : ISerializer
	{
		protected abstract void SerializeInternal(Stream stream, object instance, IDictionary options);
		protected abstract object DeserializeInternal(Stream stream, Type expectedType, IDictionary options);
		protected abstract object ConvertInternal(object instance, Type expectedType, IDictionary options);
		protected virtual string ContentTypeInternal { get; set; }

		#region Implementation of ISerializer

		void ISerializer.Serialize(Stream stream, object instance, IDictionary options) {
			this.SerializeInternal(stream, instance, options);
		}

		object ISerializer.Deserialize(Stream stream, Type expectedType, IDictionary options) {
			return this.DeserializeInternal(stream, expectedType, options);
		}

		object ISerializer.Convert(object instance, Type expectedType, IDictionary options) {
			return this.ConvertInternal(instance, expectedType, options);
		}

		string ISerializer.ContentType {
			get => this.ContentTypeInternal;
			set => this.ContentTypeInternal = value;
		}

		#endregion
	}
}

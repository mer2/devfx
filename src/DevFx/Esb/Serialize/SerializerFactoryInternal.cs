using System;
using System.Collections;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using DevFx.Logging;
using DevFx.Esb.Serialize.Settings;
using DevFx.Core;

namespace DevFx.Esb.Serialize
{
	[Object]
	internal class SerializerFactoryInternal : IInitializable<SerializerFactorySetting>, ISerializerFactory
	{
		[Autowired]
		protected IObjectService ObjectService { get; set; }
		protected Dictionary<string, ISerializer> Serializers { get; set; }
		public virtual void Init(SerializerFactorySetting setting) {
			this.Serializers = new Dictionary<string, ISerializer>();
			foreach(var item in setting.Serializers) {
				if(!item.Enabled) {
					continue;
				}
				var serializer = this.ObjectService.GetOrCreateObject<ISerializer>(item.TypeName);
				if(serializer != null) {
					serializer.ContentType = item.ContentType;
					if(setting.Debug) {
						serializer = new SerializerWrapper(serializer);
					}
					this.Serializers.Add(item.Name, serializer);
				}
			}
			this.Default = this.Serializers[setting.DefaultSerializerName];
		}

		public ISerializer GetSerializer(string contentType) {
			if(string.IsNullOrEmpty(contentType)) {
				return null;
			}
			return this.Serializers.Values.FirstOrDefault(s => contentType.StartsWith(s.ContentType, StringComparison.InvariantCultureIgnoreCase));
		}

		public ISerializer Default { get; private set; }

		private class SerializerWrapper : SerializerBase
		{
			public SerializerWrapper(ISerializer serializer) {
				this.serializer = serializer;
			}
			private readonly ISerializer serializer;

			protected override void SerializeInternal(Stream stream, object instance, IDictionary options) {
				using(var ms = new MemoryStream()) {
					this.serializer.Serialize(ms, instance, options);
					ms.WriteTo(stream);
					ms.Position = 0;
					var data = (new StreamReader(ms)).ReadToEnd();
					LogService.Debug($"Instance: {instance}, Serialized {ms.Length}: {Environment.NewLine}{data}", this);
				}
			}

			protected override object DeserializeInternal(Stream stream, Type expectedType, IDictionary options) {
				const int bufferSize = 1024;
				using(var ms = new MemoryStream()) {
					var br = new BinaryReader(stream);
					int length;
					do {
						var buffer = br.ReadBytes(bufferSize);
						length = buffer.Length;
						ms.Write(buffer, 0, length);
					} while (length >= bufferSize);
					ms.Position = 0;
					var data = new StreamReader(ms).ReadToEnd();
					LogService.Debug($"ContentType: {this.serializer.ContentType}, Type: {expectedType?.FullName}, Deserialize {ms.Length}: {Environment.NewLine}{data}", this);
					ms.Position = 0;
					return this.serializer.Deserialize(ms, expectedType, options);
				}
			}

			protected override object ConvertInternal(object instance, Type expectedType, IDictionary options) {
				return this.serializer.Convert(instance, expectedType, options);
			}

			protected override string ContentTypeInternal {
				get => this.serializer.ContentType;
				set => this.serializer.ContentType = value;
			}
		}
	}
}

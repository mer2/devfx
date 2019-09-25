using DevFx.Esb.Serialize;
using System.IO;

namespace DevFx.Cache
{
	public abstract class CacheBase : ICache
	{
		public void Add(string key, object value, ICacheDependency cacheDependency) {
			this.AddInternal(key, value, cacheDependency);
		}

		public void Add(string key, object value) {
			this.AddInternal(key, value, null);
		}

		public object Get(string key) {
			return this.GetInternal(key);
		}

		public bool TryGet(string key, out object value) {
			value = this.GetInternal(key);
			return value != null;
		}

		public void Remove(string key) {
			this.RemoveInternal(key);
		}

		public bool Contains(string key) {
			return this.ContainsInternal(key);
		}

		public void Clear() {
			this.ClearInternal();
		}

		object ICache.this[string key] {
			get => this.Get(key);
			set => this.SetInternal(key, value, null);
		}

		object ICache.this[string key, ICacheDependency cacheDependency] {
			set => this.SetInternal(key, value, cacheDependency);
		}

		public void Set(string key, object value, ICacheDependency cacheDependency) {
			this.SetInternal(key, value, cacheDependency);
		}

		public virtual int Count => -1;

		protected virtual string SerializerName { get; } = "application/json";
		private ISerializer serializer;
		protected virtual ISerializer Serializer {
			get => this.serializer ?? (this.serializer = SerializerFactory.GetSerializer(this.SerializerName));
			set => this.serializer = value;
		}

		protected virtual void Init() {
			this.Serializer = SerializerFactory.GetSerializer(this.SerializerName);
		}

		protected virtual object GetInternal(string key) {
			var data = this.GetBytesInternal(key);
			return this.DeSerialize(data);
		}

		protected virtual byte[] Serialize(object value) {
			using(var outStream = new MemoryStream()) {
				this.Serializer.Serialize(outStream, value, null);
				return outStream.ToArray();
			}
		}

		protected virtual object DeSerialize(byte[] data) {
			object result = null;
			if(data != null) {
				using(var inStream = new MemoryStream(data)) {
					result = this.Serializer.Deserialize(inStream, null, null);
				}
			}
			return result;
		}

		protected abstract void RemoveInternal(string key);
		protected abstract bool ContainsInternal(string key);
		protected abstract void ClearInternal();
		protected abstract void AddInternal(string key, object value, ICacheDependency cacheDependency);
		protected abstract void SetInternal(string key, object value, ICacheDependency cacheDependency);
		protected abstract byte[] GetBytesInternal(string key);
	}
}

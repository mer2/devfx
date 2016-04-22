/* Copyright(c) 2005-2011 R2@DevFx.NET, License(LGPL) */

using HTB.DevFx.Config;
using HTB.DevFx.Remoting;

namespace HTB.DevFx.Cache.CacheStorages
{
	/// <summary>
	/// 远程缓存存储代理类
	/// </summary>
	/// <remarks>
	///	远程缓存存储代理的配置：
	///		<code>
	///			url="远端对象的配置，例如：tcp://localhost:8080/RemoteCacheStorage"
	///		</code>
	/// </remarks>
	public class RemoteCacheStorageProxy : RemotingClientProxy<ICacheStorage>, ICacheStorage, ISettingInitialize
	{
		public RemoteCacheStorageProxy() : base(null) { }
		public RemoteCacheStorageProxy(string url) : base(url) { }

		void ISettingInitialize.Init(IConfigSetting configSetting) {
			this.RemotingUri = configSetting.GetSetting("url");
		}

		#region ICacheStorage Members

		int ICacheStorage.Count {
			get { return this.RemotingIsReady() ? this.RemotingObject.Count : -1; }
		}

		void ICacheStorage.Add(string key, ICacheItem value) {
			if (this.RemotingIsReady()) {
				this.RemotingObject.Add(key, @value);
			}
		}

		ICacheItem ICacheStorage.Get(string key) {
			return this.RemotingIsReady() ? this.RemotingObject.Get(key) : null;
		}

		ICacheItem ICacheStorage.Get(int index) {
			return this.RemotingIsReady() ? this.RemotingObject.Get(index) : null;
		}

		void ICacheStorage.Set(string key, ICacheItem value) {
			if (this.RemotingIsReady()) {
				this.RemotingObject.Set(key, @value);
			}
		}

		void ICacheStorage.Set(int index, ICacheItem value) {
			if (this.RemotingIsReady()) {
				this.RemotingObject.Set(index, @value);
			}
		}

		void ICacheStorage.Remove(string key) {
			if (this.RemotingIsReady()) {
				this.RemotingObject.Remove(key);
			}
		}

		void ICacheStorage.RemoveAt(int index) {
			if (this.RemotingIsReady()) {
				this.RemotingObject.RemoveAt(index);
			}
		}

		bool ICacheStorage.Contains(string key) {
			return this.RemotingIsReady() && this.RemotingObject.Contains(key);
		}

		void ICacheStorage.Clear() {
			if (this.RemotingIsReady()) {
				this.RemotingObject.Clear();
			}
		}

		string[] ICacheStorage.GetAllKeys() {
			return this.RemotingIsReady() ? this.RemotingObject.GetAllKeys() : null;
		}

		ICacheItem[] ICacheStorage.GetAllValues() {
			return this.RemotingIsReady() ? this.RemotingObject.GetAllValues() : null;
		}

		ICacheItem ICacheStorage.CreateCacheItem(string key, object value, ICacheDependency cacheDependency) {
			return new CacheItem(key, value, cacheDependency);
		}

		#endregion
	}
}
using System;
using System.Collections.Generic;
using DevFx.Core;

namespace DevFx.Cache
{
	public class CachedObject<T> where T : class
	{
		private readonly int interval;//检查过期的间隔（秒）
		private readonly Func<DateTime?, T, T> missingFunc;
		public CachedObject(int interval, Func<DateTime?, T, T> missingFunc) {
			this.interval = interval;
			this.missingFunc = missingFunc;
		}

		private T cachedObject;
		private DateTime lastFetchTime;
		public DateTime? LastUpdate { get; private set; }
		public T GetCachedObject() {
			var now = DateTime.Now;
			if(this.lastFetchTime.AddSeconds(this.interval) < now) {//已过期
				lock(this) {
					this.cachedObject = this.missingFunc(this.LastUpdate, this.cachedObject);
					this.lastFetchTime = now;
					this.LastUpdate = now;
				}
			}
			return this.cachedObject;
		}

		public void RefreshCachedObject(bool reset = false) {
			lock (this) {
				this.lastFetchTime = DateTime.MinValue;
				if(reset) {
					this.LastUpdate = null;
				}
			}
		}
	}

	public class CachedObjects<T> where T : IObjectIdentifier
	{
		private readonly int interval;//检查过期的间隔（秒）
		private readonly Func<DateTime?, T[]> missingFunc;
		public CachedObjects(int interval, Func<DateTime?, T[]> missingFunc) {
			this.interval = interval;
			this.missingFunc = missingFunc;
		}

		private readonly List<T> cachedObjects = new List<T>();
		private DateTime lastFetchTime;
		public DateTime? LastUpdate { get; private set; }
		public T[] GetCachedObjects() {
			var now = DateTime.Now;
			var list = this.cachedObjects;
			if(this.lastFetchTime.AddSeconds(this.interval) < now) {//已过期
				lock(this) {
					this.lastFetchTime = now;
					var objects = this.missingFunc(this.LastUpdate);
					if(objects != null && objects.Length > 0) {
						this.LastUpdate = now;
						foreach(var item in objects) {
							var id = item.GetIdentifier();
							list.RemoveAll(x => x.GetIdentifier() == id);
							list.Add(item);
						}
					}
				}
			}
			return list.ToArray();
		}

		public void RefreshCachedObjects(bool reset = false) {
			lock(this) {
				this.lastFetchTime = DateTime.MinValue;
				if (reset) {
					this.cachedObjects.Clear();
					this.LastUpdate = null;
				}
			}
		}
	}
}

/* Copyright(c) 2005-2011 R2@DevFx.NET, License(LGPL) */

using DevFx.Utils;
using System;

namespace DevFx.Cache
{
	public class Cache : TimerBase, ICache
	{
		public Cache(ICacheStorage cacheStorage = null, double interval = 1000) : base(interval) {
			if(cacheStorage == null) {
				cacheStorage = new NullCacheStorage();
			}
			this.CacheStorage = cacheStorage;
		}
		protected virtual ICacheStorage CacheStorage { get; private set; }

		#region Overrides of TimerBase

		/// <summary>
		/// 时间到了后引发的操作
		/// </summary>
		protected override void OnTimer() {
			for (var i = 0; i < this.CacheStorage.Count; ) {
				var item = (CacheItem)this.CacheStorage.Get(i);
				if (item != null && item.CacheDependency.IsExpired) {
					this.CacheStorage.RemoveAt(i);
				} else {
					i++;
				}
			}
		}

		#endregion

		#region Implementation of ICache

		/// <summary>
		/// 以健值方式获取/设置缓存项（值）
		/// </summary>
		/// <param name="key">缓存项的健值</param>
		/// <remarks>
		///	如果是设置值，则使用永不过期策略缓存
		/// </remarks>
		public virtual object this[string key] {
			get => this.Get(key);
			set => this.Add(key, value, new NullCacheDependency());
		}

		/// <summary>
		/// 按指定健值和过期策略来设置缓存项（值）
		/// </summary>
		/// <param name="key">缓存项的健值</param>
		/// <param name="cacheDependency">缓存项的过期策略</param>
		public object this[string key, ICacheDependency cacheDependency] {
			set => this.Add(key, value, cacheDependency);
		}

		public void Set(string key, object value, ICacheDependency cacheDependency) {
			this.Add(key, value, cacheDependency);
		}

		/// <summary>
		/// 获取此缓存器所缓存项的个数
		/// </summary>
		public virtual int Count => this.CacheStorage.Count;

		/// <summary>
		/// 添加一项到缓存器中
		/// </summary>
		/// <param name="key">缓存项的健值</param>
		/// <param name="value">缓存的对象</param>
		/// <param name="cacheDependency">缓存项的过期策略</param>
		public virtual void Add(string key, object value, ICacheDependency cacheDependency) {
			var item = this.CacheStorage.Get(key);
			if (item == null) {
				item = this.CacheStorage.CreateCacheItem(key, value, cacheDependency);
			} else {
				item.Value = value;
			}
			this.CacheStorage.Set(key, item);
			this.StartTimer();
		}

		/// <summary>
		/// 添加一项到缓存器中
		/// </summary>
		/// <param name="key">缓存项的健值</param>
		/// <param name="value">缓存的对象</param>
		/// <remarks>
		/// 没有指定过期策略，则使用永不过期策略缓存
		/// </remarks>
		public void Add(string key, object value) {
			this.Add(key, value, new NullCacheDependency());
		}

		/// <summary>
		/// 获取缓存项
		/// </summary>
		/// <param name="key">缓存项的健值</param>
		/// <returns>缓存的对象，如果缓存中没有命中，则返回<c>null</c></returns>
		public virtual object Get(string key) {
			var item = this.CacheStorage.Get(key);
			object value = null;
			if (item != null) {
				if (!item.CacheDependency.IsExpired) {
					value = item.Value;
					item.Hits++;
					item.LastAccessTime = DateTime.Now;
					item.CacheDependency.Reset();
				} else {
					this.CacheStorage.Remove(key);
				}
			}
			return value;
		}

		/// <summary>
		/// 尝试获取缓存项，如果存在则返回<c>true</c>
		/// </summary>
		/// <param name="key">缓存项的健值</param>
		/// <param name="value">>缓存的对象，如果缓存中没有命中，则返回<c>null</c></param>
		/// <returns>如果存在则返回<c>true</c></returns>
		public virtual bool TryGet(string key, out object value) {
			value = this.Get(key);
			return value != null;
		}

		/// <summary>
		/// 移除缓存项
		/// </summary>
		/// <param name="key">缓存项的健值</param>
		public virtual void Remove(string key) {
			this.CacheStorage.Remove(key);
		}

		/// <summary>
		/// 判断缓存器中是否包含指定健值的缓存项
		/// </summary>
		/// <param name="key">缓存项的健值</param>
		/// <returns>是/否</returns>
		public virtual bool Contains(string key) {
			return this.CacheStorage.Contains(key);
		}

		/// <summary>
		/// 清除此缓存器中所有的项
		/// </summary>
		/// <remarks>
		/// 不影响配置文件中其他缓存器
		/// </remarks>
		public virtual void Clear() {
			this.CacheStorage.Clear();
		}

		#endregion
	}
}

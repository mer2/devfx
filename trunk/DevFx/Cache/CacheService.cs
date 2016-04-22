/* Copyright(c) 2005-2011 R2@DevFx.NET, License(LGPL) */

using System.Linq;
using System.Collections.Generic;
using HTB.DevFx.Cache.Config;
using HTB.DevFx.Core;

namespace HTB.DevFx.Cache
{
	public class CacheService : ServiceBase<ICacheServiceSetting>, ICacheService
	{
		protected internal CacheService() {
		}

		protected virtual Dictionary<string, ICacheInternal> Caches { get; set; }

		protected override void OnInit() {
			base.OnInit();
			if (this.Setting.Caches != null && this.Setting.Caches.Length > 0) {
				this.Caches = this.Setting.Caches.Select( x => {
					var cache = this.ObjectService.GetOrCreateObject<ICacheInternal>(x.TypeName);
					cache.Init(x);
					return cache;
				}).ToDictionary(k => k.CacheName, v => v);
			}
		}

		/// <summary>
		/// 获取已配置的缓存器
		/// </summary>
		/// <param name="cacheName">配置文件中配置的缓存器名称</param>
		/// <returns>ICache的实例</returns>
		protected virtual ICache GetCacheInternal(string cacheName) {
			ICacheInternal cache;
			this.Caches.TryGetValue(cacheName, out cache);
			return cache;
		}

		/// <summary>
		/// 获取缓存项值
		/// </summary>
		/// <param name="cacheName">配置文件中配置的缓存器名称</param>
		/// <param name="key">缓存项健值</param>
		/// <returns>缓存项值，如果没有命中，则返回<c>null</c></returns>
		protected object GetCacheValueInternal(string cacheName, string key) {
			return this.GetCacheValueInternal(cacheName, key, false);
		}

		/// <summary>
		/// 获取缓存项值
		/// </summary>
		/// <param name="cacheName">配置文件中配置的缓存器名称</param>
		/// <param name="key">缓存项健值</param>
		/// <param name="throwOnError">如果有错误，是否抛出异常</param>
		/// <returns>缓存项值，如果没有命中，则返回<c>null</c></returns>
		protected object GetCacheValueInternal(string cacheName, string key, bool throwOnError) {
			var cache = this.GetCacheInternal(cacheName);
			object value = null;
			if (cache == null) {
				if (throwOnError) {
					throw new CacheException("没有配置Cache：" + cacheName);
				}
			} else {
				value = cache[key];
			}

			return value;
		}

		#region Implementation of ICacheService

		/// <summary>
		/// 获取已配置的缓存器
		/// </summary>
		/// <param name="cacheName">配置文件中配置的缓存器名称</param>
		/// <returns>ICache的实例</returns>
		ICache ICacheService.GetCache(string cacheName) {
			return this.GetCacheInternal(cacheName);
		}

		/// <summary>
		/// 获取缓存项值
		/// </summary>
		/// <param name="cacheName">配置文件中配置的缓存器名称</param>
		/// <param name="key">缓存项健值</param>
		/// <returns>缓存项值，如果没有命中，则返回<c>null</c></returns>
		object ICacheService.GetCacheValue(string cacheName, string key) {
			return this.GetCacheValueInternal(cacheName, key);
		}

		/// <summary>
		/// 获取缓存项值
		/// </summary>
		/// <param name="cacheName">配置文件中配置的缓存器名称</param>
		/// <param name="key">缓存项健值</param>
		/// <param name="throwOnError">如果有错误，是否抛出异常</param>
		/// <returns>缓存项值，如果没有命中，则返回<c>null</c></returns>
		object ICacheService.GetCacheValue(string cacheName, string key, bool throwOnError) {
			return this.GetCacheValueInternal(cacheName, key, throwOnError);
		}

		#endregion

		#region ICacheService Static Members

		public static ICacheService Current {
			get { return DevFx.ObjectService.GetObject<ICacheService>(); }
		}

		public static ICache GetCache(string cacheName) {
			return Current.GetCache(cacheName);
		}

		public static object GetCacheValue(string cacheName, string key) {
			return Current.GetCacheValue(cacheName, key);
		}

		public static object GetCacheValue(string cacheName, string key, bool throwOnError) {
			return Current.GetCacheValue(cacheName, key, throwOnError);
		}

		#endregion
	}
}

/* Copyright(c) 2005-2011 R2@DevFx.NET, License(LGPL) */

using System;
using System.IO;

namespace DevFx.Cache
{
	/// <summary>
	/// 缓存依赖基类
	/// </summary>
	[Serializable]
	public abstract class CacheDependency : ICacheDependency
	{
		/// <summary>
		/// 是否已过期
		/// </summary>
		protected abstract bool IsExpired { get; }

		/// <summary>
		/// 重置缓存策略（相当于重新开始缓存）
		/// </summary>
		protected abstract void Reset();

		#region ICacheDependency Members

		bool ICacheDependency.IsExpired {
			get { return this.IsExpired; }
		}

		void ICacheDependency.Reset() {
			this.Reset();
		}

		#endregion

		#region CacheDependency Static Members

		/// <summary>
		/// 创建永不过期的缓存策略
		/// </summary>
		/// <returns><see cref="ICacheDependency"/></returns>
		public static ICacheDependency Create() {
			return new NullCacheDependency();
		}

		/// <summary>
		/// 创建绝对时间过期的缓存策略
		/// </summary>
		/// <param name="absoluteExpiration">绝对时间</param>
		/// <returns><see cref="ICacheDependency"/></returns>
		public static ICacheDependency Create(DateTime absoluteExpiration) {
			return new ExpirationCacheDependency(absoluteExpiration);
		}

		/// <summary>
		/// 创建相对时间过期的缓存策略
		/// </summary>
		/// <param name="slidingExpiration">相对时间</param>
		/// <returns><see cref="ICacheDependency"/></returns>
		public static ICacheDependency Create(TimeSpan slidingExpiration) {
			return new ExpirationCacheDependency(slidingExpiration);
		}

		/// <summary>
		/// 创建文件依赖方式的过期策略
		/// </summary>
		/// <param name="fileName">需要监视的文件名</param>
		/// <returns><see cref="ICacheDependency"/></returns>
		public static ICacheDependency Create(string fileName) {
			return new FileCacheDependency(fileName);
		}

		/// <summary>
		/// 创建文件依赖方式的过期策略
		/// </summary>
		/// <param name="fileName">需要监视的文件名</param>
		/// <param name="filters">监视方式</param>
		/// <returns><see cref="ICacheDependency"/></returns>
		public static ICacheDependency Create(string fileName, NotifyFilters filters) {
			return new FileCacheDependency(fileName, filters);
		}

		#endregion
	}
}

/* Copyright(c) 2005-2011 R2@DevFx.NET, License(LGPL) */

using System;

namespace HTB.DevFx.Cache
{
	/// <summary>
	/// 缓存项的包装类
	/// </summary>
	[Serializable]
	public class CacheItem : ICacheItem
	{
		/// <summary>
		/// 构造函数
		/// </summary>
		/// <param name="key">缓存项健值</param>
		/// <param name="value">储存项值</param>
		/// <param name="cacheDependency">过期策略</param>
		public CacheItem(string key, object @value, ICacheDependency cacheDependency) {
			this.Key = key;
			this.Value = @value;
			this.CacheDependency = cacheDependency;
			this.Hits = 0;
			this.LastAccessTime = DateTime.Now;
		}

		/// <summary>
		/// 获取过期策略
		/// </summary>
		public ICacheDependency CacheDependency { get; private set; }

		/// <summary>
		/// 获取健值
		/// </summary>
		public string Key { get; private set; }

		/// <summary>
		/// 获取缓存项
		/// </summary>
		public object Value { get; set; }

		/// <summary>
		/// 命中次数
		/// </summary>
		public int Hits { get; set; }

		/// <summary>
		/// 最后命中时间
		/// </summary>
		public DateTime LastAccessTime { get; set; }
	}
}

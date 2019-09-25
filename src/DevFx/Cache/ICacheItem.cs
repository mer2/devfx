/* Copyright(c) 2005-2011 R2@DevFx.NET, License(LGPL) */

using System;

namespace DevFx.Cache
{
	public interface ICacheItem
	{
		/// <summary>
		/// 获取过期策略
		/// </summary>
		ICacheDependency CacheDependency { get; }

		/// <summary>
		/// 获取健值
		/// </summary>
		string Key { get; }

		/// <summary>
		/// 缓存项
		/// </summary>
		object Value { get; set; }

		/// <summary>
		/// 命中次数
		/// </summary>
		int Hits { get; set; }

		/// <summary>
		/// 最后命中时间
		/// </summary>
		DateTime LastAccessTime { get; set; }
	}
}

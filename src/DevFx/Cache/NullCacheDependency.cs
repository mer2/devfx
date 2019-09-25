/* Copyright(c) 2005-2011 R2@DevFx.NET, License(LGPL) */

using System;

namespace DevFx.Cache
{
	/// <summary>
	/// 永不过期的缓存策略
	/// </summary>
	[Serializable]
	public class NullCacheDependency : CacheDependency
	{
		/// <summary>
		/// 是否已过期（永远返回<c>false</c>）
		/// </summary>
		protected override bool IsExpired {
			get { return false; }
		}

		/// <summary>
		/// 重置缓存策略（相当于重新开始缓存）
		/// </summary>
		protected override void Reset() {
		}
	}
}

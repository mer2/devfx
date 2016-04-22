/* Copyright(c) 2005-2011 R2@DevFx.NET, License(LGPL) */

namespace HTB.DevFx.Cache
{
	/// <summary>
	/// 缓存过期策略接口
	/// </summary>
	public interface ICacheDependency
	{
		/// <summary>
		/// 是否已过期
		/// </summary>
		bool IsExpired { get; }

		/// <summary>
		/// 重置缓存策略（相当于重新开始缓存）
		/// </summary>
		void Reset();
	}
}

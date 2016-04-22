/* Copyright(c) 2005-2011 R2@DevFx.NET, License(LGPL) */

namespace HTB.DevFx.Data.Entities
{
	/// <summary>
	/// 数据缓存动作
	/// </summary>
	public enum CacheAction
	{
		/// <summary>
		/// 进行数据缓存（如果数据不存在）
		/// </summary>
		Cache,

		/// <summary>
		/// 从缓存中移除数据
		/// </summary>
		Remove
	}
}

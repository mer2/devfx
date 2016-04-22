/* Copyright(c) 2005-2011 R2@DevFx.NET, License(LGPL) */

namespace HTB.DevFx.Core
{
	/// <summary>
	/// 对象需要配置信息的接口
	/// </summary>
	public interface ISettingRequired
	{
		/// <summary>
		/// 设置对象的配置信息
		/// </summary>
		object Setting { set; }
	}
}

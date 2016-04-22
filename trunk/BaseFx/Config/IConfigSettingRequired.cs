/* Copyright(c) 2005-2011 R2@DevFx.NET, License(LGPL) */

namespace HTB.DevFx.Config
{
	/// <summary>
	/// 需要通用配置节的显式定义
	/// </summary>
	public interface IConfigSettingRequired
	{
		/// <summary>
		/// 配置节
		/// </summary>
		IConfigSetting ConfigSetting { get; }
	}
}

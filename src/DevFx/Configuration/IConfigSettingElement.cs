/* Copyright(c) 2005-2011 R2@DevFx.NET, License(LGPL) */

namespace DevFx.Configuration
{
	/// <summary>
	/// 强类型配置接口
	/// </summary>
	public interface IConfigSettingElement
	{
		/// <summary>
		/// 获取或设置此强类型对应的配置节
		/// </summary>
		IConfigSetting ConfigSetting { get; set; }
	}
}

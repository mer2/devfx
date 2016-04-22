/* Copyright(c) 2005-2011 R2@DevFx.NET, License(LGPL) */

using HTB.DevFx.Config;

namespace HTB.DevFx.Core
{
	/// <summary>
	/// 配置处理接口
	/// </summary>
	public interface ISettingHandler
	{
		/// <summary>
		/// 获取处理后的配置
		/// </summary>
		/// <param name="setting">处理前的配置</param>
		/// <returns>处理后的配置</returns>
		IConfigSettingElement GetSetting(IConfigSettingElement setting);
	}
}
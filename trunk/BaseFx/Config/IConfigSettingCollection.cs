/* Copyright(c) 2005-2011 R2@DevFx.NET, License(LGPL) */

namespace HTB.DevFx.Config
{
	/// <summary>
	/// 配置节集合接口
	/// </summary>
	public interface IConfigSettingCollection
	{
		/// <summary>
		/// 添加配置节
		/// </summary>
		/// <param name="setting">配置节</param>
		/// <returns>配置节</returns>
		IConfigSetting Add(IConfigSetting setting);

		/// <summary>
		/// 添加/替换配置节（如果存在则替换）
		/// </summary>
		/// <param name="setting">配置节</param>
		IConfigSetting Set(IConfigSetting setting);

		/// <summary>
		/// 深度复制集合
		/// </summary>
		/// <param name="parent">父配置节</param>
		/// <returns>复制后的集合</returns>
		IConfigSettingCollection Clone(IConfigSetting parent);

		/// <summary>
		/// 获取集合内容
		/// </summary>
		IConfigSetting[] Values { get; }
	}
}

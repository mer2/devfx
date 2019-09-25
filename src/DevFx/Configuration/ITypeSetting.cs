/* Copyright(c) 2005-2017 R2@DevFx.NET, License(LGPL) */

namespace DevFx.Configuration
{
	/// <summary>
	/// 类型配置
	/// </summary>
	public interface ITypeSetting : INameSetting
	{
		/// <summary>
		/// 类型名称
		/// </summary>
		[SettingName("type")]
		string TypeName { get; }
	}
}

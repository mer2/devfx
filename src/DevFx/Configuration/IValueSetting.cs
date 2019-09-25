/* Copyright(c) 2005-2017 R2@DevFx.NET, License(LGPL) */

namespace DevFx.Configuration
{
	/// <summary>
	/// 配置值
	/// </summary>
	public interface IValueSetting : ITypeSetting
	{
		/// <summary>
		/// 值
		/// </summary>
		string Value { get; }
	}
}

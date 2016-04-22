/* Copyright(c) 2005-2011 R2@DevFx.NET, License(LGPL) */

namespace HTB.DevFx.Core.Config
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

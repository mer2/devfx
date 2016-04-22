/* Copyright(c) 2005-2011 R2@DevFx.NET, License(LGPL) */

using HTB.DevFx.Config;
using HTB.DevFx.Core;
using HTB.DevFx.Core.Config;

namespace HTB.DevFx.Esb.Config
{
	/// <summary>
	/// 对象配置接口
	/// </summary>
	public interface IObjectSetting : ITypeSetting, IConfigSettingRequired, IObjectContext
	{
		/// <summary>
		/// 被映射到的对象
		/// </summary>
		string MapTo { get; }

		/// <summary>
		/// 对象创建者
		/// </summary>
		string Builder { get; }

		/// <summary>
		/// 对象配置信息
		/// </summary>
		IValueSetting SettingSet { get; }
	}
}
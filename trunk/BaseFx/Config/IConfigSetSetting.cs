/* Copyright(c) 2005-2011 R2@DevFx.NET, License(LGPL) */

namespace HTB.DevFx.Config
{
	/// <summary>
	/// 配置节集合的配置信息
	/// </summary>
	public interface IConfigSetSetting
	{
		/// <summary>
		/// 配置节集合动作标签
		/// </summary>
		string Tag { get; }

		/// <summary>
		/// 配置节集合动作类型
		/// </summary>
		ConfigSettingOperator Operator { get; }
	
		/// <summary>
		/// 配置节集合键值属性名
		/// </summary>
		string KeyName { get; }
		/// <summary>
		/// 配置节集合键值是否允许空
		/// </summary>
		bool KeyNullable { get; }

		/// <summary>
		/// 原始配置信息
		/// </summary>
		string Source { get; }
	}
}

/* Copyright(c) 2005-2011 R2@DevFx.NET, License(LGPL) */

namespace HTB.DevFx.Config
{
	/// <summary>
	/// 配置节命令
	/// </summary>
	public enum ConfigSettingOperator
	{
		/// <summary>
		/// 一般命令（即无动作）
		/// </summary>
		None = 0,

		/// <summary>
		/// 添加配置节
		/// </summary>
		Add = 1,

		/// <summary>
		/// 移除配置节
		/// </summary>
		Remove,

		/// <summary>
		/// 移动配置节
		/// </summary>
		Move,

		/// <summary>
		/// 清除所有配置节
		/// </summary>
		Clear,

		/// <summary>
		/// 更新（合并）配置节（如果不存在，则忽略此命令）
		/// </summary>
		Update,

		/// <summary>
		/// 设置配置节，如果存在则合并，否则添加
		/// </summary>
		Set,
	}
}
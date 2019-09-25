/* Copyright(c) 2005-2011 R2@DevFx.NET, License(LGPL) */

namespace DevFx.Configuration
{
	/// <summary>
	/// 配置节来源的配置信息
	/// </summary>
	public interface IConfigSourceSetting
	{
		/// <summary>
		/// 配置节内容来源文件
		/// </summary>
		string File { get; }
		/// <summary>
		/// 配置节内容来源文件中的节点
		/// </summary>
		string Node { get; }
		/// <summary>
		/// 配置节内容提供者
		/// </summary>
		string Provider { get; }

		/// <summary>
		/// 原始的配置信息
		/// </summary>
		string Source { get; }
	}
}

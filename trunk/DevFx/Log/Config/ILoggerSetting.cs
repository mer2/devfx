/* Copyright(c) 2005-2011 R2@DevFx.NET, License(LGPL) */

namespace HTB.DevFx.Log.Config
{
	public interface ILoggerSetting
	{
		/// <summary>
		/// 此日志记录器记录级别的最小值
		/// </summary>
		int MinLevel { get; }

		/// <summary>
		/// 此日志记录器记录级别的最大值
		/// </summary>
		int MaxLevel { get; }

		/// <summary>
		/// 此日志记录器的优先级
		/// </summary>
		int Priority { get; }

		/// <summary>
		/// 此日志记录器是否有效
		/// </summary>
		bool Enabled { get; }

		/// <summary>
		/// 日志记录器实例
		/// </summary>
		ILogger Logger { get; }
	}
}

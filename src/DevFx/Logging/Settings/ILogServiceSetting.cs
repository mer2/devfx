/* Copyright(c) 2005-2011 R2@DevFx.NET, License(LGPL) */

namespace DevFx.Logging.Settings
{
	[Service]
	public interface ILogServiceSetting
	{
		/// <summary>
		/// 是否启用日志模块
		/// </summary>
		bool Enabled { get; }
		/// <summary>
		/// 定时写入日志毫秒数
		/// </summary>
		double Interval { get; }
		/// <summary>
		/// 需要被处理的日志最低等级
		/// </summary>
		int Verbose { get; }
		/// <summary>
		/// 内部缓存大小
		/// </summary>
		int QueueCount { get; }

		ILoggerSetting[] Loggers { get; }
	}
}

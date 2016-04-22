/* Copyright(c) 2005-2011 R2@DevFx.NET, License(LGPL) */

namespace HTB.DevFx.Log.Config
{
	public interface ILogServiceSetting
	{
		bool Enabled { get; }
		double Interval { get; }
		/// <summary>
		/// 需要被处理的日志最低等级
		/// </summary>
		int Verbose { get; }
		ILoggerSetting[] Loggers { get; }
	}
}

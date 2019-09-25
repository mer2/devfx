/* Copyright(c) 2005-2011 R2@DevFx.NET, License(LGPL) */

using System;

namespace DevFx.Logging
{
	/// <summary>
	/// 日志服务接口
	/// </summary>
	[Service]
	public interface ILogService
	{
		/// <summary>
		/// 日志写入事件
		/// </summary>
		event Action<LogItem[]> LogsWriting;

		/// <summary>
		/// 写日志
		/// </summary>
		/// <param name="message">日志消息</param>
		/// <param name="level">日志等级，参见<see cref="LogLevel"/></param>
		/// <param name="category">日志分类</param>
		/// <param name="innerObject">附加的内部对象，比如<see cref="Exception"/></param>
		void WriteLog(string message, int level = LogLevel.INFO, object category = null, object innerObject = null);

		void Debug(string message, object category = null, object innerObject = null);
		void Info(string message, object category = null, object innerObject = null);
		void Error(string message, object category = null, object innerObject = null);
	}
}

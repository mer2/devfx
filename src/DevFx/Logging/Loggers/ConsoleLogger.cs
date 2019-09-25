/* Copyright(c) 2005-2011 R2@DevFx.NET, License(LGPL) */

using System;

namespace DevFx.Logging.Loggers
{
	/// <summary>
	/// 以控制台输出方式的日志记录器
	/// </summary>
	/// <remarks>
	/// 提供给控制台应用程序使用
	/// </remarks>
	public class ConsoleLogger : LoggerBase
	{
		protected override void WriteLogsInternal(LogItem[] items) {
			if(Environment.UserInteractive) {
				var msg = LogHelper.FormatLog(items, false);
				Console.Write(msg);
			}
		}
	}
}
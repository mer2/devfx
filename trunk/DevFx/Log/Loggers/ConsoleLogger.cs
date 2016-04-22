/* Copyright(c) 2005-2011 R2@DevFx.NET, License(LGPL) */

using System;

namespace HTB.DevFx.Log.Loggers
{
	/// <summary>
	/// 以控制台输出方式的日志记录器
	/// </summary>
	/// <remarks>
	/// 提供给控制台应用程序使用
	/// </remarks>
	public class ConsoleLogger : LoggerBase
	{
		#region Overrides of LoggerBase

		/// <summary>
		/// 实际的日志写入处理过程
		/// </summary>
		/// <param name="sender">调用者</param>
		/// <param name="args"><see cref="LogEventArgs"/> 数组</param>
		protected override void WriteLogInternal(object sender, LogEventArgs[] args) {
			var msg = this.LogFormat(sender, args, false);
			if (Environment.UserInteractive) {
				Console.Write(msg);
			}
		}

		#endregion
	}
}
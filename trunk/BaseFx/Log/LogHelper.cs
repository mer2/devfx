/* Copyright(c) 2005-2011 R2@DevFx.NET, License(LGPL) */

using System;
using System.IO;
using System.Text;
using HTB.DevFx.Esb;
using HTB.DevFx.Utils;

namespace HTB.DevFx.Log
{
	/// <summary>
	/// 日志帮助类
	/// </summary>
	public abstract class LogHelper
	{
		/// <summary>
		/// 格式化日志信息
		/// </summary>
		/// <param name="sender">调用者</param>
		/// <param name="args"><see cref="LogEventArgs"/> 数组</param>
		/// <param name="withBatchFlag">批量标志</param>
		/// <returns>格式化后的日志信息</returns>
		public static string FormatLog(object sender, LogEventArgs[] args, bool withBatchFlag) {
			if (args == null || args.Length <= 0) {
				return string.Format("{0}", sender);
			}
			var log = new StringBuilder();
			if (withBatchFlag) {
				log.AppendFormat("[###### Start ######]\r\n{0}\r\n", sender);
			}
			foreach (var e in args) {
				log.AppendFormat("[{0}][{1}][{2}]\r\n{3}\r\n{4}\r\n------------------\r\n", e.LogTime, e.Sender, LogLevel.Format(e.Level), e.Message, e.InnerObject);
			}
			if (withBatchFlag) {
				log.Append("[###### End   ######]\r\n");
			}
			return log.ToString();
		}

		/// <summary>
		/// 写日志
		/// </summary>
		/// <param name="sender">调用者</param>
		/// <param name="level">日志等级，参见<see cref="LogLevel"/></param>
		/// <param name="logFormat">日志格式</param>
		/// <param name="parameters">格式化参数</param>
		/// <remarks>先找是否有<see cref="ILogService"/>的实现，有则调用相关方法；无则默认写入WARN级及以上的日志信息到应用程序同级目录Logs下</remarks>
		public static void WriteLog(object sender, int level, string logFormat, params object[] parameters) {
			var service = ServiceLocator.GetService<ILogService>();
			if (service != null) {
				service.WriteLog(sender, level, logFormat, parameters);
			} else {
				if(level >= LogLevel.WARN) {
					try {
						var arg = new LogEventArgs(sender, level, DateTime.Now, string.Format(logFormat, parameters));
						FileHelper.WriteLog(FileHelper.GetFullPath(string.Format(@"..{0}Logs{0}Error", Path.DirectorySeparatorChar)), string.Format(@"{0}yyyy{0}MM{0}dd{0}HH\.\l\o\g", Path.DirectorySeparatorChar), FormatLog(sender, new[] { arg }, false));
					} catch { }
				}
			}
		}
	}
}

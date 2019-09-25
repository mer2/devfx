/* Copyright(c) 2005-2011 R2@DevFx.NET, License(LGPL) */

using DevFx.Utils;
using System;
using System.IO;
using System.Text;

namespace DevFx.Logging
{
	/// <summary>
	/// 日志帮助类
	/// </summary>
	internal abstract class LogHelper
	{
		/// <summary>
		/// 格式化日志信息
		/// </summary>
		/// <param name="items"><see cref="LogItem"/> 数组</param>
		/// <param name="withBatchFlag">批量标志</param>
		/// <returns>格式化后的日志信息</returns>
		public static string FormatLog(LogItem[] items, bool withBatchFlag) {
			if (items == null || items.Length <= 0) {
				return null;
			}
			var batchFlag = withBatchFlag ? Guid.NewGuid().ToString("N") : null;
			var log = new StringBuilder();
			if (withBatchFlag) {
				log.AppendFormat("[###### Start ######]{0}{1}", batchFlag, Environment.NewLine);
			}
			foreach (var item in items) {
				log.AppendFormat("[{0}][{1}][{2}]{4}{3}{4}", item.IssueTime, item.Category, LogLevel.Format(item.Level), item.Message, Environment.NewLine);
				if(item.InnerObject != null) {
					log.AppendFormat("{0}{1}", item.InnerObject, Environment.NewLine);
				}
				log.AppendFormat("------------------{0}", Environment.NewLine);
			}
			if (withBatchFlag) {
				log.AppendFormat("[######  End  ######]{0}{1}", batchFlag, Environment.NewLine);
			}
			return log.ToString();
		}

		/// <summary>
		/// 写日志
		/// </summary>
		/// <param name="message">日志内容</param>
		/// <param name="level">日志等级，参见<see cref="LogLevel"/></param>
		/// <param name="category">日志分类</param>
		/// <remarks>先找是否有<see cref="ILogService"/>的实现，有则调用相关方法；无则默认写入WARN级及以上的日志信息到应用程序同级目录Logs下</remarks>
		public static void WriteLog(string message, int level, string category) {
			var service = LogService.Current;
			if (service != null) {
				service.WriteLog(message, level, category, level);
			} else {
				if(level >= LogLevel.WARN) {
					try {
						var log = new LogItem(category, level, DateTime.Now, message);
						FileHelper.WriteLog(FileHelper.GetFullPath(string.Format(@"..{0}logs{0}error", Path.DirectorySeparatorChar)), string.Format(@"{0}yyyy{0}MM{0}dd{0}HH\.\l\o\g", Path.DirectorySeparatorChar), FormatLog(new[] { log }, false));
					} catch { }
				}
			}
		}
	}
}

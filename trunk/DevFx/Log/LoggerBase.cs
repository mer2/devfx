/* Copyright(c) 2005-2011 R2@DevFx.NET, License(LGPL) */

using HTB.DevFx.Utils;

namespace HTB.DevFx.Log
{
	/// <summary>
	/// 日志记录处理器（基类）
	/// </summary>
	public abstract class LoggerBase : ILogger
	{
		/// <summary>
		/// 日志写入处理
		/// </summary>
		/// <param name="sender">调用者</param>
		/// <param name="args"><see cref="LogEventArgs"/> 数组</param>
		protected virtual void WriteLog(object sender, LogEventArgs[] args) {
			if(Checker.CheckEmptyArray("args", args, false)) {
				return;
			}
			this.WriteLogInternal(sender, args);
		}

		/// <summary>
		/// 日志格式化
		/// </summary>
		/// <param name="sender">调用者</param>
		/// <param name="args"><see cref="LogEventArgs"/> 数组</param>
		/// <param name="withBatchFlag">是否加上批量日志的标志</param>
		/// <returns>格式化后的日志</returns>
		protected virtual string LogFormat(object sender, LogEventArgs[] args, bool withBatchFlag) {
			return LogHelper.FormatLog(sender, args, withBatchFlag);
		}

		/// <summary>
		/// 实际的日志写入处理过程
		/// </summary>
		/// <param name="sender">调用者</param>
		/// <param name="args"><see cref="LogEventArgs"/> 数组</param>
		protected abstract void WriteLogInternal(object sender, LogEventArgs[] args);

		#region ILogger Members

		void ILogger.WriteLog(object sender, LogEventArgs[] args) {
			this.WriteLog(sender, args);
		}

		#endregion
	}
}

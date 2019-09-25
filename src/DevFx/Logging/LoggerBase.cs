/* Copyright(c) 2005-2011 R2@DevFx.NET, License(LGPL) */

namespace DevFx.Logging
{
	/// <summary>
	/// 日志记录处理器（基类）
	/// </summary>
	public abstract class LoggerBase : ILogger
	{
		/// <summary>
		/// 日志写入处理
		/// </summary>
		/// <param name="items"><see cref="LogItem"/> 数组</param>
		protected virtual void WriteLogs(LogItem[] items) {
			if(items == null || items.Length <= 0) {
				return;
			}
			this.WriteLogsInternal(items);
		}

		/// <summary>
		/// 实际的日志写入处理过程
		/// </summary>
		/// <param name="items"><see cref="LogItem"/> 数组</param>
		protected abstract void WriteLogsInternal(LogItem[] items);

		#region ILogger Members

		void ILogger.WriteLogs(LogItem[] items) {
			this.WriteLogsInternal(items);
		}

		#endregion
	}
}

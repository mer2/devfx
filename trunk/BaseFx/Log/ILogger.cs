/* Copyright(c) 2005-2011 R2@DevFx.NET, License(LGPL) */

namespace HTB.DevFx.Log
{
	/// <summary>
	/// 日志记录器接口
	/// </summary>
	public interface ILogger
	{
		/// <summary>
		/// 写入日志（批量）
		/// </summary>
		/// <param name="sender">调用者</param>
		/// <param name="args"><see cref="LogEventArgs"/> 数组</param>
		void WriteLog(object sender, LogEventArgs[] args);
	}
}

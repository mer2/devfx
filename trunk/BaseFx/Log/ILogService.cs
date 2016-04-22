/* Copyright(c) 2005-2011 R2@DevFx.NET, License(LGPL) */

using System;
using HTB.DevFx.Core;

namespace HTB.DevFx.Log
{
	/// <summary>
	/// 日志服务接口
	/// </summary>
	public interface ILogService
	{
		/// <summary>
		/// 日志写入事件
		/// </summary>
		event EventHandlerDelegates<LogEventArgs> LogWriting;

		/// <summary>
		/// 写日志
		/// </summary>
		/// <param name="message">日志消息</param>
		void WriteLog(string message);

		/// <summary>
		/// 写日志
		/// </summary>
		/// <param name="sender">调用者</param>
		/// <param name="message">日志消息</param>
		void WriteLog(object sender, string message);

		/// <summary>
		/// 写日志
		/// </summary>
		/// <param name="logFormat">日志格式</param>
		/// <param name="parameters">格式化参数</param>
		void WriteLog(string logFormat, params object[] parameters);

		/// <summary>
		/// 写日志
		/// </summary>
		/// <param name="sender">调用者</param>
		/// <param name="logFormat">日志格式</param>
		/// <param name="parameters">格式化参数</param>
		void WriteLog(object sender, string logFormat, params object[] parameters);

		/// <summary>
		/// 写日志
		/// </summary>
		/// <param name="sender">调用者</param>
		/// <param name="level">日志等级，参见<see cref="LogLevel"/></param>
		/// <param name="logFormat">日志格式</param>
		/// <param name="parameters">格式化参数</param>
		void WriteLog(object sender, int level, string logFormat, params object[] parameters);

		/// <summary>
		/// 写日志
		/// </summary>
		/// <param name="sender">调用者</param>
		/// <param name="level">日志等级，参见<see cref="LogLevel"/></param>
		/// <param name="message">日志消息</param>
		void WriteLog(object sender, int level, string message);

		/// <summary>
		/// 写日志
		/// </summary>
		/// <param name="sender">调用者</param>
		/// <param name="level">日志等级，参见<see cref="LogLevel"/></param>
		/// <param name="message">日志消息</param>
		/// <param name="innerObject">附加的内部对象，比如<see cref="Exception"/></param>
		void WriteLog(object sender, int level, string message, object innerObject);
	}
}

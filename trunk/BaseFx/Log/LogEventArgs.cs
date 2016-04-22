/* Copyright(c) 2005-2011 R2@DevFx.NET, License(LGPL) */

using System;
using HTB.DevFx.Core;

namespace HTB.DevFx.Log
{
	/// <summary>
	/// 日志写入事件参数类
	/// </summary>
	[Serializable]
	public class LogEventArgs : EventArgsBase
	{
		private readonly int level;
		private readonly string message;
		private readonly DateTime logTime;
		
		/// <summary>
		/// 构造方法
		/// </summary>
		/// <param name="sender">日志发送者</param>
		/// <param name="level">日志等级，参见<see cref="LogLevel"/></param>
		/// <param name="logTime">日志发生时间</param>
		/// <param name="message">日志消息</param>
		public LogEventArgs(object sender, int level, DateTime logTime, string message) : base(sender) {
			this.level = level;
			this.logTime = logTime;
			this.message = message;
		}

		/// <summary>
		/// 构造方法
		/// </summary>
		/// <param name="sender">日志发送者</param>
		/// <param name="level">日志等级，参见<see cref="LogLevel"/></param>
		/// <param name="logTime">日志发生时间</param>
		/// <param name="message">日志消息</param>
		/// <param name="innerObject">附加的内部对象，比如<see cref="Exception"/></param>
		public LogEventArgs(object sender, int level, DateTime logTime, string message, object innerObject) : this(sender, level, logTime, message) {
			this.InnerObject = innerObject;
		}

		/// <summary>
		/// 日志等级，参见<see cref="LogLevel"/>
		/// </summary>
		public int Level {
			get { return this.level; }
		}

		/// <summary>
		/// 日志发生时间
		/// </summary>
		public DateTime LogTime {
			get { return this.logTime; }
		}

		/// <summary>
		/// 日志消息
		/// </summary>
		public string Message {
			get { return this.message; }
		}

		/// <summary>
		/// 是否已处理
		/// </summary>
		public bool Handled { get; set; }

		/// <summary>
		/// 附加的内部对象，比如<see cref="Exception"/>
		/// </summary>
		public object InnerObject { get; set; }
	}
}

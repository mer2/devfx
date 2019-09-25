using System;

namespace DevFx.Logging
{
	[Serializable]
	public class LogItem
	{
		/// <summary>
		/// 构造方法
		/// </summary>
		/// <param name="category">日志分类</param>
		/// <param name="level">日志等级，参见<see cref="LogLevel"/></param>
		/// <param name="issueTime">日志发生时间</param>
		/// <param name="message">日志消息</param>
		/// <param name="innerObject">附加对象</param>
		public LogItem(string category, int level, DateTime issueTime, string message, object innerObject = null) {
			this.Category = category;
			this.Level = level;
			this.IssueTime = issueTime;
			this.Message = message;
			this.InnerObject = innerObject;
		}

		/// <summary>
		/// 日志分类
		/// </summary>
		public string Category { get; }
		/// <summary>
		/// 日志等级，参见<see cref="LogLevel"/>
		/// </summary>
		public int Level { get; }
		/// <summary>
		/// 日志发生时间
		/// </summary>
		public DateTime IssueTime { get; }
		/// <summary>
		/// 日志消息
		/// </summary>
		public string Message { get; }
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

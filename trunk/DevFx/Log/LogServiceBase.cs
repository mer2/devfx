/* Copyright(c) 2005-2011 R2@DevFx.NET, License(LGPL) */

using System;
using System.Collections.Generic;
using System.Diagnostics;
using HTB.DevFx.Core;
using HTB.DevFx.Log.Loggers;
using HTB.DevFx.Utils;

namespace HTB.DevFx.Log
{
	/// <summary>
	/// 日志服务基类
	/// </summary>
	public abstract class LogServiceBase : TimerBase, ILogService, IDisposable
	{
		/// <summary>
		/// 构造方法，初始化<see cref="LogQueue"/>
		/// </summary>
		protected LogServiceBase() {
			this.LogQueue = new Queue<LogEventArgs>(1024);
		}

		private ILogger defaultHandler;
		private readonly object lockHandler = new object();

		/// <summary>
		/// 缺省的日志处理器
		/// </summary>
		protected virtual ILogger DefaultHandler {
			get {
				if (this.defaultHandler == null) {
					lock (this.lockHandler) {
						if (this.defaultHandler == null) {
							this.defaultHandler = new FileLogger();
						}
					}
				}
				return this.defaultHandler;
			}
			set { this.defaultHandler = value; }
		}

		/// <summary>
		/// 把异常信息写入缺省的日志处理器中
		/// </summary>
		/// <param name="e">异常</param>
		/// <param name="args">日志列表</param>
		protected virtual void WriteErrorToDefaultHandler(Exception e, LogEventArgs[] args) {
			if (this.DefaultHandler != null) {
				try {
					var arg = new LogEventArgs(this, LogLevel.EMERGENCY, DateTime.Now, e.Message, LogHelper.FormatLog(e, args, true));
					this.DefaultHandler.WriteLog(this, new[] { arg });
				} catch { }
			}
		}

		/// <summary>
		/// 时间到了后（引发写入日志）的操作
		/// </summary>
		protected override void OnTimer() {
			var args = new List<LogEventArgs>();
			if (this.LogQueue.Count > 0) {
				lock (this.LogQueue) {
					while (this.LogQueue.Count > 0) {
						var ex = this.LogQueue.Dequeue();
						args.Add(ex);
					}
				}
			}
			if (!this.Enabled || args.Count <= 0) {
				return;
			}

			var argArray = args.ToArray();
			try {
				this.OnLogWriting(argArray);
			} catch(Exception ex) {
				this.WriteErrorToDefaultHandler(ex, argArray);
			}
		}

		/// <summary>
		/// 是否实际写入日志
		/// </summary>
		protected virtual bool Enabled { get; set; }

		/// <summary>
		/// 日志队列
		/// </summary>
		protected virtual Queue<LogEventArgs> LogQueue { get; private set; }

		/// <summary>
		/// 日志事件
		/// </summary>
		protected virtual event EventHandlerDelegates<LogEventArgs> LogWritingInternal;

		/// <summary>
		/// 日志写入触发方法
		/// </summary>
		/// <param name="e">LogEventArgs[]</param>
		protected virtual void OnLogWriting(LogEventArgs[] e) {
			if (this.LogWritingInternal != null) {
				this.LogWritingInternal(this, e);
			}
		}

		/// <summary>
		/// 把日志压入堆栈并启动定时器
		/// </summary>
		/// <param name="sender">调用者</param>
		/// <param name="level">日志等级，参见<see cref="LogLevel"/></param>
		/// <param name="message">日志消息</param>
		/// <param name="innerObject">附加的内部对象，比如<see cref="Exception"/></param>
		protected virtual void WriteLogInternal(object sender, int level, string message, object innerObject) {
			sender = this.GetLogSource(sender);
			var e = new LogEventArgs(sender, level, DateTime.Now, message, innerObject);
			lock (this.LogQueue) {
				this.LogQueue.Enqueue(e);
			}
			this.StartTimer();
		}

		/// <summary>
		/// 日志来源将从系统堆栈中获取
		/// </summary>
		/// <param name="sender">调用者</param>
		/// <returns>调用者</returns>
		protected virtual object GetLogSource(object sender) {
			if(sender == null) {
				var t = new StackTrace();
				var f = t.GetFrame(t.FrameCount - 1);
				return f.GetMethod();
			}
			return sender;
		}

		#region ILogService Members

		event EventHandlerDelegates<LogEventArgs> ILogService.LogWriting {
			add {
				this.LogWritingInternal -= value;
				this.LogWritingInternal += value;
			}
			remove { this.LogWritingInternal -= value; }
		}

		void ILogService.WriteLog(string message) {
			this.WriteLogInternal(null, LogLevel.INFO, message, null);
		}

		void ILogService.WriteLog(object sender, string message) {
			this.WriteLogInternal(sender, LogLevel.INFO, message, null);
		}

		void ILogService.WriteLog(string logFormat, params object[] parameters) {
			this.WriteLogInternal(null, LogLevel.INFO, string.Format(logFormat, parameters), null);
		}

		void ILogService.WriteLog(object sender, string logFormat, params object[] parameters) {
			this.WriteLogInternal(sender, LogLevel.INFO, string.Format(logFormat, parameters), null);
		}

		void ILogService.WriteLog(object sender, int level, string logFormat, params object[] parameters) {
			this.WriteLogInternal(sender, level, string.Format(logFormat, parameters), null);
		}

		void ILogService.WriteLog(object sender, int level, string message) {
			this.WriteLogInternal(sender, level, message, null);
		}

		void ILogService.WriteLog(object sender, int level, string message, object innerObject) {
			this.WriteLogInternal(sender, level, message, innerObject);
		}

		#endregion

		#region IDisposable Members

		private bool disposed;
		protected virtual void Dispose() {
			if(!this.disposed) {
				this.disposed = true;
				this.OnTimer();
			}
		}

		void IDisposable.Dispose() {
			this.Dispose();
			GC.SuppressFinalize(this);
		}

		~LogServiceBase() {
			this.Dispose();
		}

		#endregion
	}
}

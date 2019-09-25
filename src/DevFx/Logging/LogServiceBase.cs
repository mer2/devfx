/* Copyright(c) 2005-2011 R2@DevFx.NET, License(LGPL) */

using DevFx.Core;
using DevFx.Logging.Loggers;
using DevFx.Logging.Settings;
using DevFx.Utils;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DevFx.Logging
{
	/// <summary>
	/// 日志服务基类
	/// </summary>
	public abstract class LogServiceBase : ILogService, IInitializable<ILogServiceSetting>
	{
		private TimerBase timer;
		private ILogger defaultHandler;
		private readonly object lockHandler = new object();
		protected ILogServiceSetting Setting { get; set; }
		protected Queue<LogItem> LogQueue { get; set; }
		protected virtual event Action<LogItem[]> LogsWritingInternal;
		protected virtual ILoggerSetting[] LogWriters { get; set; }

		[Autowired]
		protected virtual IObjectService ObjectService { get; set; }

		protected virtual void Init(ILogServiceSetting setting) {
			this.Setting = setting;
			this.LogQueue = new Queue<LogItem>(setting.QueueCount);
			this.timer = TimerBase.CreateDefaultTimer(setting.Interval, this.OnTimer);

			var logWriters = this.Setting.Loggers;
			if (logWriters != null) {
				logWriters = logWriters.Where(x => x.Enabled).OrderByDescending(x => x.Priority).ToArray();
				foreach (var wr in logWriters) {
					wr.Logger = this.ObjectService.GetOrCreateObject<ILogger>(wr.LoggerTypeName);
				}
			}
			this.LogWriters = logWriters;
		}

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
		/// <param name="items">日志列表</param>
		protected virtual void WriteErrorToDefaultHandler(Exception e, LogItem[] items) {
			if (this.DefaultHandler != null) {
				try {
					var log = new LogItem(nameof(Logging), LogLevel.EMERGENCY, DateTime.Now, e.Message, LogHelper.FormatLog(items, true));
					this.DefaultHandler.WriteLogs(new[] { log });
				} catch { }
			}
		}

		/// <summary>
		/// 时间到了后（引发写入日志）的操作
		/// </summary>
		protected virtual void OnTimer() {
			var logs = new List<LogItem>();
			var queue = this.LogQueue;
			if (queue.Count > 0) {
				lock (queue) {
					while (queue.Count > 0) {
						var ex = queue.Dequeue();
						logs.Add(ex);
					}
				}
			}
			if (!this.Setting.Enabled || logs.Count <= 0) {
				return;
			}

			var logArray = logs.ToArray();
			try {
				this.OnLogsWriting(logArray);
			} catch(Exception ex) {
				this.WriteErrorToDefaultHandler(ex, logArray);
			}
		}

		/// <summary>
		/// 日志写入触发方法
		/// </summary>
		/// <param name="items">LogItem 数组</param>
		protected virtual void OnLogsWriting(LogItem[] items) {
			foreach (var writer in this.LogWriters) {
				if (writer.Enabled) {
					var wr = writer;
					var list = items.Where(e => e.Level >= wr.MinLevel && e.Level <= wr.MaxLevel).ToArray();
					if(list.Length > 0) {
						writer.Logger?.WriteLogs(list);
					}
				}
			}
			this.LogsWritingInternal?.Invoke(items);
		}

		/// <summary>
		/// 把日志压入堆栈并启动定时器
		/// </summary>
		/// <param name="message">日志消息</param>
		/// <param name="level">日志等级，参见<see cref="LogLevel"/></param>
		/// <param name="category">日志分类</param>
		/// <param name="innerObject">附加的内部对象，比如<see cref="Exception"/></param>
		protected virtual void WriteLogInternal(string message, int level, string category, object innerObject) {
			if (level < this.Setting.Verbose) {
				return;
			}
			var e = new LogItem(category, level, DateTime.Now, message, innerObject);
			lock (this.LogQueue) {
				this.LogQueue.Enqueue(e);
			}
			this.timer.StartTimer();
		}

		protected virtual string GetCategoryFromObject(object category) {
			if(category == null) {
				return null;
			}
			if(category is string) {
				return (string)category;
			}
			if(category is ValueType) {
				return category.ToString();
			}
			return category.GetType().FullName;
		}

		#region Implementation of IInitializable

		void IInitializable<ILogServiceSetting>.Init(ILogServiceSetting setting) {
			this.Init(setting);
		}

		#endregion

		#region ILogService Members

		event Action<LogItem[]> ILogService.LogsWriting {
			add {
				this.LogsWritingInternal -= value;
				this.LogsWritingInternal += value;
			}
			remove { this.LogsWritingInternal -= value; }
		}

		void ILogService.WriteLog(string message, int level, object category, object innerObject) {
			this.WriteLogInternal(message, level, this.GetCategoryFromObject(category), innerObject);
		}

		void ILogService.Debug(string message, object category, object innerObject) {
			this.WriteLogInternal(message, LogLevel.DEBUG, this.GetCategoryFromObject(category), innerObject);
		}

		void ILogService.Info(string message, object category, object innerObject) {
			this.WriteLogInternal(message, LogLevel.INFO, this.GetCategoryFromObject(category), innerObject);
		}

		void ILogService.Error(string message, object category, object innerObject) {
			this.WriteLogInternal(message, LogLevel.ERROR, this.GetCategoryFromObject(category), innerObject);
		}

		#endregion
	}
}

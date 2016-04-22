/* Copyright(c) 2005-2011 R2@DevFx.NET, License(LGPL) */

using System;
using System.Linq;
using HTB.DevFx.Core;
using HTB.DevFx.Log.Config;

namespace HTB.DevFx.Log
{
	public class LogService : LogServiceBase, IService, IInitializable<ILogServiceSetting>
	{
		protected internal LogService() {
		}

		protected virtual ILogServiceSetting Setting { get; set; }
		protected virtual ILoggerSetting[] LogWriters { get; set; }

		private void LogServiceLogWritingInternal(object sender, LogEventArgs[] args) {
			foreach(var writer in this.LogWriters) {
				if (writer.Enabled) {
					var wr = writer;
					writer.Logger.WriteLog(sender, args.Where(e => e.Level >= wr.MinLevel && e.Level <= wr.MaxLevel).ToArray());
				}
			}
		}

		protected override double Interval {
			get { return this.Setting.Interval; }
		}

		protected virtual void Init(ILogServiceSetting setting) {
			this.Setting = setting;
			this.Enabled = this.Setting.Enabled;
			var logWriters = this.Setting.Loggers;
			if (logWriters != null) {
				logWriters.OrderByDescending(x => x.Priority);
			}
			this.LogWriters = logWriters;

			this.LogWritingInternal -= this.LogServiceLogWritingInternal;
			this.LogWritingInternal += this.LogServiceLogWritingInternal;
		}

		protected override void WriteLogInternal(object sender, int level, string message, object innerObject) {
			if(level < this.Setting.Verbose) {
				return;
			}
			base.WriteLogInternal(sender, level, message, innerObject);
		}

		#region Implementation of IInitializable

		void IInitializable<ILogServiceSetting>.Init(ILogServiceSetting setting) {
			this.Init(setting);
		}

		#endregion

		#region static members

		/// <summary>
		/// 写日志
		/// </summary>
		/// <param name="message">日志消息</param>
		public static void WriteLog(string message) {
			WriteLog((object)null, LogLevel.INFO, message);
		}

		/// <summary>
		/// 写日志
		/// </summary>
		/// <param name="sender">调用者</param>
		/// <param name="message">日志消息</param>
		public static void WriteLog(object sender, string message) {
			WriteLog(sender, LogLevel.INFO, message);
		}

		/// <summary>
		/// 写日志
		/// </summary>
		/// <param name="logFormat">日志格式</param>
		/// <param name="parameters">格式化参数</param>
		public static void WriteLog(string logFormat, params object[] parameters) {
			WriteLog(string.Format(logFormat, parameters));
		}

		/// <summary>
		/// 写日志
		/// </summary>
		/// <param name="sender">调用者</param>
		/// <param name="logFormat">日志格式</param>
		/// <param name="parameters">格式化参数</param>
		public static void WriteLog(object sender, string logFormat, params object[] parameters) {
			WriteLog(sender, LogLevel.INFO, string.Format(logFormat, parameters));
		}

		/// <summary>
		/// 写日志
		/// </summary>
		/// <param name="sender">调用者</param>
		/// <param name="level">日志等级，参见<see cref="LogLevel"/></param>
		/// <param name="logFormat">日志格式</param>
		/// <param name="parameters">格式化参数</param>
		public static void WriteLog(object sender, int level, string logFormat, params object[] parameters) {
			WriteLog(sender, level, string.Format(logFormat, parameters));
		}

		/// <summary>
		/// 写日志
		/// </summary>
		/// <param name="sender">调用者</param>
		/// <param name="level">日志等级，参见<see cref="LogLevel"/></param>
		/// <param name="message">日志消息</param>
		public static void WriteLog(object sender, int level, string message) {
			WriteLog(sender, level, message, (object)null);
		}

		/// <summary>
		/// 写日志
		/// </summary>
		/// <param name="sender">调用者</param>
		/// <param name="level">日志等级，参见<see cref="LogLevel"/></param>
		/// <param name="message">日志消息</param>
		/// <param name="innerObject">附加的内部对象，比如<see cref="Exception"/></param>
		public static void WriteLog(object sender, int level, string message, object innerObject) {
			Current.WriteLog(sender, level, message, innerObject);
		}

		/// <summary>
		/// 写入日志的事件
		/// </summary>
		public static event EventHandlerDelegates<LogEventArgs> LogWriting {
			add { Current.LogWriting += value; }
			remove { Current.LogWriting -= value; }
		}

		public static ILogService Current {
			get { return ObjectService.GetObject<ILogService>(); }
		}

		#endregion static members
	}
}

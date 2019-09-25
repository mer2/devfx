/* Copyright(c) 2005-2011 R2@DevFx.NET, License(LGPL) */

using System;

namespace DevFx.Logging
{
	[Object]
	public class LogService : LogServiceBase
	{
		protected internal LogService() {
		}

		internal void OnTimerInternal() {
			this.OnTimer();
		}

		#region static members

		/// <summary>
		/// 写日志
		/// </summary>
		/// <param name="message">日志消息</param>
		/// <param name="level">日志等级，参见<see cref="LogLevel"/></param>
		/// <param name="category">日志分类</param>
		/// <param name="innerObject">附加的内部对象，比如<see cref="Exception"/></param>
		public static void WriteLog(string message, int level = LogLevel.INFO, object category = null, object innerObject = null) {
			Current.WriteLog(message, level, category, innerObject);
		}

		public static void Debug(string message, object category = null, object innerObject = null) {
			Current.Debug(message, category, innerObject);
		}

		public static void Info(string message, object category = null, object innerObject = null) {
			Current.Info(message, category, innerObject);
		}

		public static void Error(string message, object category = null, object innerObject = null) {
			Current.Error(message, category, innerObject);
		}

		private static ILogService current;
		public static ILogService Current => current ?? (current = DevFx.ObjectService.GetObject<ILogService>());

		#endregion static members
	}
}

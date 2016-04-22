/* Copyright(c) 2005-2011 R2@DevFx.NET, License(LGPL) */

using System;

namespace HTB.DevFx.Log
{
	/// <summary>
	/// 日志等级（系统预置）
	/// </summary>
	/// <remarks>
	/// 日志等级一般是由应用程序自己定义的，本类只是提供一般的等级分类，应用程序也可以不遵循此分类
	/// </remarks>
	public static class LogLevel
	{
		/// <summary>
		/// 等级最大值
		/// </summary>
		public const int MAX = Int32.MaxValue;

		/// <summary>
		/// 紧急事件的日志等级（120000）
		/// </summary>
		public const int EMERGENCY = 120000;

		/// <summary>
		/// 致命事件的日志等级（110000）
		/// </summary>
		public const int FATAL = 110000;

		/// <summary>
		///  警报事件的日志等级（100000）
		/// </summary>
		public const int ALERT = 100000;

		/// <summary>
		/// 错误事件的日志等级（70000）
		/// </summary>
		public const int ERROR = 70000;

		/// <summary>
		/// 警告事件的日志等级（60000）
		/// </summary>
		public const int WARN = 60000;

		/// <summary>
		/// 通知事件的日志等级（50000）
		/// </summary>
		public const int NOTICE = 50000;

		/// <summary>
		/// 信息事件的日志等级（40000）
		/// </summary>
		public const int INFO = 40000;

		/// <summary>
		/// 调试事件的日志等级（30000）
		/// </summary>
		public const int DEBUG = 30000;

		/// <summary>
		/// 跟踪事件的日志等级（20000）
		/// </summary>
		public const int TRACE = 20000;

		/// <summary>
		/// 不表示任何等级（-1）
		/// </summary>
		public const int NA = -1;

		/// <summary>
		/// 最小等级
		/// </summary>
		public const int MIN = Int32.MinValue;

		/// <summary>
		/// 从名称获取等级
		/// </summary>
		/// <param name="levelName">日志等级名称</param>
		/// <returns>等级代码</returns>
		public static int Parse(string levelName) {
			if(levelName == null) {
				throw new LogException("级别名称为Null");
			}
			var level = Parse(levelName, NA);
			if(level == NA) {
				throw new LogException("没有发现指定名称的级别");
			}
			return level;
		}

		/// <summary>
		/// 从名称获取等级
		/// </summary>
		/// <param name="levelName">日志等级名称</param>
		/// <param name="defaultValue">如果没找到，缺省的等级代码</param>
		/// <returns>等级代码</returns>
		public static int Parse(string levelName, int defaultValue) {
			return Parse(levelName, defaultValue, true);
		}

		/// <summary>
		/// 从名称获取等级
		/// </summary>
		/// <param name="levelName">日志等级名称</param>
		/// <param name="defaultValue">如果没找到，缺省的等级代码</param>
		/// <param name="changeType">如果没找到，尝试转换成等级代码</param>
		/// <returns>等级代码</returns>
		public static int Parse(string levelName, int defaultValue, bool changeType) {
			if (string.IsNullOrEmpty(levelName)) {
				return defaultValue;
			}
			var level = defaultValue;
			switch (levelName.ToUpper()) {
				case "MAX":
					level = MAX;
					break;
				case "EMERGENCY":
					level = EMERGENCY;
					break;
				case "FATAL":
					level = FATAL;
					break;
				case "ALERT":
					level = ALERT;
					break;
				case "ERROR":
					level = ERROR;
					break;
				case "WARN":
					level = WARN;
					break;
				case "NOTICE":
					level = NOTICE;
					break;
				case "INFO":
					level = INFO;
					break;
				case "DEBUG":
					level = DEBUG;
					break;
				case "TRACE":
					level = TRACE;
					break;
				case "MIN":
					level = MIN;
					break;
				default:
					if(changeType) {
						try {
							level = (int)Convert.ChangeType(levelName, typeof(int));
						} catch { }
					}
					break;
			}
			return level;
		}

		/// <summary>
		/// 尝试从名称获取等级
		/// </summary>
		/// <param name="levelName">日志等级名称</param>
		/// <param name="levelValue">传入的等级代码</param>
		/// <returns>是否成功获取等级代码</returns>
		public static bool TryParse(string levelName, ref int levelValue) {
			var level = Parse(levelName, NA);
			switch(level) {
				case NA:
					return false;
				default:
					levelValue = level;
					return true;
			}
		}

		/// <summary>
		/// 从日志等级获取预定义的名称
		/// </summary>
		/// <param name="level">日志等级</param>
		/// <returns>等级名称</returns>
		public static string ParseToName(int level) {
			switch(level) {
				case MAX:
					return "MAX";
				case EMERGENCY:
					return "EMERGENCY";
				case FATAL:
					return "FATAL";
				case ALERT:
					return "ALERT";
				case ERROR:
					return "ERROR";
				case WARN:
					return "WARN";
				case NOTICE:
					return "NOTICE";
				case INFO:
					return "INFO";
				case DEBUG:
					return "DEBUG";
				case TRACE:
					return "TRACE";
				case MIN:
					return "MIN";
				default:
					return "UNKNOWN";
			}
		}

		/// <summary>
		/// 从日志等级获取预定义的格式化名称
		/// </summary>
		/// <param name="level">日志等级</param>
		/// <param name="format">格式化</param>
		/// <returns>格式化后的等级名称</returns>
		public static string ParseToName(int level, string format) {
			return string.Format(format, level, ParseToName(level));
		}

		/// <summary>
		/// 从日志等级获取预定义的格式化名称
		/// </summary>
		/// <param name="level">日志等级</param>
		/// <returns>格式化后的等级名称</returns>
		public static string Format(int level) {
			return ParseToName(level, "[{0}]{1}");
		}
	}
}

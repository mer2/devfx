/* Copyright(c) 2005-2011 R2@DevFx.NET, License(LGPL) */

using DevFx.Utils;
using System;
using System.IO;

namespace DevFx.Logging.Loggers
{
	/// <summary>
	/// 文件日志记录器
	/// </summary>
	public class FileLogger : LoggerBase
	{
		/// <summary>
		/// 构造方法
		/// </summary>
		public FileLogger() : this("../logs/", "{now:yyyyMMdd}_{serverName}_{directoryName}.log") {
		}

		/// <summary>
		/// 构造方法
		/// </summary>
		/// <param name="logPath">日志记录的基路径</param>
		/// <param name="logFile">日志记录的文件名格式（包括相对路径）</param>
		public FileLogger(string logPath, string logFile) {
			this.Init(logPath, logFile);
		}

		protected void Init(string logPath, string logFile) {
			this.LogPath = logPath;
			this.LogFile = logFile;
		}

		///<summary>
		/// 日志记录的基路径
		///</summary>
		public virtual string LogPath { get; protected set; }
		/// <summary>
		/// 日志记录的文件名格式（包括相对路径）
		/// </summary>
		public virtual string LogFile { get; protected set; }

		protected virtual string GetFileName(string input, Func<string, string> filter = null) {
			//需要替换{now}为当前时间
			var now = DateTime.Now;
			//需要替换{serverName}为当前服务器名，目前为当前服务器IP
			var serverName = Environment.MachineName;
			if(filter != null) {
				serverName = filter(serverName);
			}
			//需要替换{directoryName}为当前目录名
			var appPath = AppDomain.CurrentDomain.BaseDirectory;
			var directoryName = string.Empty;
			var directory = new DirectoryInfo(appPath);
			if(directory.Root.Name != directory.Name) {
				directoryName = directory.Name;
			}
			if(filter != null) {
				directoryName = filter(directoryName);
			}
			var filePath = input.NamedStringFormat(new { now, serverName, directoryName });
			return FileHelper.GetFullPath(filePath);
		}

		/// <summary>
		/// 日志写入处理
		/// </summary>
		/// <param name="items"><see cref="LogItem"/></param>
		protected override void WriteLogsInternal(LogItem[] items) {
			FileHelper.WriteLog(this.GetFileName(this.LogPath + this.LogFile), LogHelper.FormatLog(items, false));
		}
	}
}
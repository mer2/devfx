/* Copyright(c) 2005-2011 R2@DevFx.NET, License(LGPL) */

using System;
using System.IO;
using HTB.DevFx.Utils;

namespace HTB.DevFx.Log.Loggers
{
	/// <summary>
	/// 文件日志记录器
	/// </summary>
	public class FileLogger : LoggerBase
	{
		/// <summary>
		/// 构造方法
		/// </summary>
		public FileLogger() : this(string.Format(".{0}Logs{0}", Path.DirectorySeparatorChar), string.Format(@"{0}{{0:yyyy}}{0}{{0:MM}}{0}{{0:dd}}{0}{{0:HH}}.txt", Path.DirectorySeparatorChar)) {
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
			//需要替换{0}为当前时间
			var now = DateTime.Now;
			//需要替换{1}为当前服务器名，目前为当前服务器IP
			var serverName = Environment.MachineName;
			if(filter != null) {
				serverName = filter(serverName);
			}
			//需要替换{2}为当前目录名
			var appPath = AppDomain.CurrentDomain.SetupInformation.ApplicationBase;
			var directoryName = string.Empty;
			var directory = new DirectoryInfo(appPath);
			if(directory.Root.Name != directory.Name) {
				directoryName = directory.Name;
			}
			if(filter != null) {
				directoryName = filter(directoryName);
			}
			var filePath = string.Format(input, now, serverName, directoryName);
			return FileHelper.GetFullPath(filePath);
		}

		/// <summary>
		/// 日志写入处理
		/// </summary>
		/// <param name="sender">调用者</param>
		/// <param name="args"><see cref="LogEventArgs"/> 数组</param>
		protected override void WriteLogInternal(object sender, LogEventArgs[] args) {
			FileHelper.WriteLog(this.GetFileName(this.LogPath + this.LogFile), this.LogFormat(sender, args, false));
		}
	}
}
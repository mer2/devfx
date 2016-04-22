/* Copyright(c) 2005-2011 R2@DevFx.NET, License(LGPL) */

using System.Text;

namespace HTB.DevFx.Log.Loggers
{
	public class RawTextLogger : FileLogger
	{
		protected RawTextLogger(string logPath, string logFile) : base(logPath, logFile) {}

		protected override string LogFormat(object sender, LogEventArgs[] args, bool withBatchFlag) {
			var message = new StringBuilder();
			foreach(var arg in args) {
				message.AppendLine(arg.Message);
			}
			return message.ToString();
		}
	}
}
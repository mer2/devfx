/* Copyright(c) 2005-2011 R2@DevFx.NET, License(LGPL) */

using System.Diagnostics;
using System.ServiceProcess;
using HTB.DevFx.Log;
using HTB.DevFx.Services;

namespace HTB.DevFx.ServiceRunners
{
	internal partial class MainService : ServiceBase
	{
		public MainService(string serviceName) {
			InitializeComponent();
			this.ServiceName = serviceName;
		}

		protected override void OnStart(string[] args) {
			LogService.LogWriting += this.LogHelperLogEvent;
			ServiceRunnerHost.Current.Start();
		}

		protected override void OnStop() {
			ServiceRunnerHost.Current.Stop();
			LogService.LogWriting -= this.LogHelperLogEvent;
		}

		private void LogHelperLogEvent(object sender, LogEventArgs[] args) {
			foreach (var e in args) {
				if(e.Level >= LogLevel.ERROR) {
					var log = string.Format("[{0}][{1}][{2}]\r\n{3}\r\n------------------\r\n", e.LogTime, e.Sender, e.Level, e.Message);
					this.EventLog.WriteEntry(log, EventLogEntryType.Error);
				}
			}
		}
	}
}
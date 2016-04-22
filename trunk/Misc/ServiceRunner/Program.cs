/* Copyright(c) 2005-2011 R2@DevFx.NET, License(LGPL) */

using System;
using System.ServiceProcess;
using System.Windows.Forms;
using HTB.DevFx.Config;
using HTB.DevFx.ServiceRunners.Config;
using HTB.DevFx.Services;
using HTB.DevFx.Utils;

namespace HTB.DevFx.ServiceRunners
{
	internal static class Program
	{
		private static string logPath;

		[STAThread]
		private static void Main(string[] args) {
			logPath = FileHelper.GetFullPath(@".\Logs\");

			AppDomain.CurrentDomain.UnhandledException += ((sender, e) => FileHelper.WriteLog(logPath, @"\c\r\a\s\h\E\x\c\e\p\t\i\o\n\.\t\x\t", string.Format("[{0:yyyy-MM-dd HH:mm:ss.fff}]\r\n{1}", DateTime.Now, e.ExceptionObject)));

			var runAs = string.Empty;
			var serviceName = string.Empty;

			if(args.Length >= 2) {
				for(int i = 0; i < args.Length; i++) {
					string[] runParamter = args[i].Split(':');
					if(runParamter.Length == 2) {
						if(runParamter[0].StartsWith("-runAs", true, null)) {
							runAs = runParamter[1];
						} else if(runParamter[0].StartsWith("-serviceName", true, null)) {
							serviceName = runParamter[1];
						}
					}
				}
			}

			var setting = ObjectService.GetObjectSetting<ServiceRunnerHost>().ToSetting<ServiceRunnerHostSetting>();
			if(string.IsNullOrEmpty(runAs)) {
				runAs = setting.RunAs;
			}

			if(string.Compare(runAs, "WinService", StringComparison.InvariantCultureIgnoreCase) == 0) {
				ServiceBase.Run(new MainService(serviceName));
			} else {
				Application.EnableVisualStyles();
				Application.SetCompatibleTextRenderingDefault(false);
				var mainForm = new MainForm {
					Text = setting.Title
				};
				Application.Run(mainForm);
			}
		}
	}
}
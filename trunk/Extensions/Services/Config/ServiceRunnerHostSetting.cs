/* Copyright(c) 2005-2011 R2@DevFx.NET, License(LGPL) */

using System;
using HTB.DevFx.Config;
[assembly: ConfigResource("res://HTB.DevFx.Services.Config.htb.devfx.services.config", Index = 0)]

namespace HTB.DevFx.Services.Config
{
	internal class ServiceRunnerHostSetting : ConfigSettingElement, IServiceRunnerHostSetting
	{
		protected override void OnConfigSettingChanged() {
			base.OnConfigSettingChanged();
			this.ServiceRunners = this.GetSettings<ServiceRunnerSetting>(null).ToArray();
		}

		public IServiceRunnerSetting[] ServiceRunners { get; private set; }

		internal class ServiceRunnerSetting : ConfigSettingElement, IServiceRunnerSetting
		{
			protected override void OnConfigSettingChanged() {
				this.Title = this.GetSetting("title");
				this.ServiceName = this.GetRequiredSetting("name");
				this.Enabled = this.GetSetting("enabled", true);
				this.ServiceType = this.GetSetting("type");
				this.Handler = this.GetSetting("handler");
			}

			public string Title { get; private set; }
			public string ServiceName { get; private set; }
			public bool Enabled { get; private set; }
			public string ServiceType { get; private set; }
			public string Handler { get; private set; }
		}

		internal class TimerRunnerSetting : ConfigSettingElement
		{
			protected override void OnConfigSettingChanged() {
				this.Interval = this.GetSetting("interval", 1000);
			}

			public double Interval { get; private set; }
		}

		internal class ScheduleRunnerSetting : ConfigSettingElement
		{
			protected override void OnConfigSettingChanged() {
				this.StartTime = this.GetSetting("startTime", DateTime.Now);
				this.IntervalType = this.GetSetting("intervalType", ScheduleRunner.DateIntervalType.Day);
				this.IntervalValue = this.GetSetting("intervalValue", 1);
			}

			public DateTime StartTime { get; private set; }
			public ScheduleRunner.DateIntervalType IntervalType { get; private set; }
			public int IntervalValue { get; private set; }
		}

		internal class RemotingRunnerSetting : ConfigSettingElement
		{
			protected override void OnConfigSettingChanged() {
				this.ConfigFile = this.GetSetting("configFile");
			}

			public string ConfigFile { get; private set; }
		}
	}
}

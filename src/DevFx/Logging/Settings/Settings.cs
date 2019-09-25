using DevFx.Configuration;
[assembly: ConfigResource("res://DevFx.Logging.Settings.Settings.config", Index = 10)]

namespace DevFx.Logging.Settings
{
	[SettingObject("~/logging", Required = true)]
	internal class LogServiceSetting : ConfigSettingElement, ILogServiceSetting
	{
		protected override void OnConfigSettingChanged() {
			this.Enabled = this.GetSetting("enabled", true);
			this.Interval = this.GetSetting("interval", 1000D);
			var levelName = this.GetSetting<string>("verbose");
			this.Verbose = LogLevel.Parse(levelName, LogLevel.INFO, true);
			this.Loggers = this.GetSettings<LoggerSetting>(null).ToArray();
		}

		public bool Enabled { get; private set; }
		public double Interval { get; private set; }
		public int Verbose { get; private set; }
		public int QueueCount { get; private set; }
		public ILoggerSetting[] Loggers { get; private set; }
	}

	internal class LoggerSetting : ConfigSettingElement, ILoggerSetting
	{
		protected override void OnConfigSettingChanged() {
			this.LoggerTypeName = this.GetSetting<string>("type");

			var levelName = this.GetSetting<string>("minLevel");
			this.MinLevel = LogLevel.Parse(levelName, LogLevel.MIN, true);
			levelName = this.GetSetting<string>("maxLevel");
			this.MaxLevel = LogLevel.Parse(levelName, LogLevel.MAX, true);

			this.Priority = this.GetSetting("priority", 0);
			this.Enabled = this.GetSetting("enabled", true);
		}

		public string LoggerTypeName { get; private set; }
		public int MinLevel { get; private set; }
		public int MaxLevel { get; private set; }
		public int Priority { get; private set; }
		public bool Enabled { get; private set; }
		public ILogger Logger { get; set; }
	}
}

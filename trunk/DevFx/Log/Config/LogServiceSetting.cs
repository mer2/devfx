/* Copyright(c) 2005-2011 R2@DevFx.NET, License(LGPL) */

using HTB.DevFx.Config;

[assembly: ConfigResource("res://HTB.DevFx.Log.Config.htb.devfx.log.config", Index = 200)]

namespace HTB.DevFx.Log.Config
{
	internal class LogServiceSetting : ConfigSettingElement, ILogServiceSetting
	{
		protected override void OnConfigSettingChanged() {
			this.Enabled = this.GetSetting("enabled", false);
			this.Interval = this.GetSetting("interval", 1000D);
			var levelName = this.GetSetting("verbose");
			this.Verbose = LogLevel.Parse(levelName, LogLevel.INFO, true);
			this.Loggers = this.GetSettings<LoggerSetting>(null).ToArray();
		}

		public bool Enabled { get; private set; }
		public double Interval { get; private set; }
		public int Verbose { get; private set; }

		public ILoggerSetting[] Loggers { get; private set; }

		internal class LoggerSetting : ConfigSettingElement, ILoggerSetting
		{
			private int? minLevel;
			private int? maxLevel;
			private int? priority;
			private bool? enabled;
			private ILogger logger;

			#region Implementation of ILoggerSetting

			/// <summary>
			/// 此日志记录器记录级别的最小值
			/// </summary>
			public int MinLevel {
				get {
					if (this.minLevel == null) {
						var levelName = this.GetSetting("minLevel");
						this.minLevel = LogLevel.Parse(levelName, LogLevel.MIN, true);
					}
					return this.minLevel.Value;
				}
			}

			/// <summary>
			/// 此日志记录器记录级别的最大值
			/// </summary>
			public virtual int MaxLevel {
				get {
					if (this.maxLevel == null) {
						var levelName = this.GetSetting("maxLevel");
						this.maxLevel = LogLevel.Parse(levelName, LogLevel.MAX, true);
					}
					return this.maxLevel.Value;
				}
			}

			/// <summary>
			/// 此日志记录器的优先级
			/// </summary>
			public int Priority {
				get {
					if (this.priority == null) {
						this.priority = this.GetSetting<int>("priority");
					}
					return this.priority.Value;
				}
			}

			/// <summary>
			/// 此日志记录器是否有效
			/// </summary>
			public bool Enabled {
				get {
					if (this.enabled == null) {
						this.enabled = this.GetSetting("enabled", true);
					}
					return this.enabled.Value;
				}
			}

			/// <summary>
			/// 日志记录器实例
			/// </summary>
			public ILogger Logger {
				get { return this.logger ?? (this.logger = this.GetObject<ILogger>("value")); }
			}

			#endregion
		}
	}
}

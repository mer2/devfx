/* Copyright(c) 2005-2011 R2@DevFx.NET, License(LGPL) */

using HTB.DevFx.Config;
[assembly: ConfigResource("res://HTB.DevFx.Pooling.Config.htb.devfx.pool.config", Index = 11)]

namespace HTB.DevFx.Pooling.Config
{
	internal class PoolLifetimeContainerSetting : ConfigSettingElement, IPoolLifetimeContainerSetting
	{
		protected override void OnConfigSettingChanged() {
			this.Enabled = this.GetSetting("enabled", true);
			this.Debug = this.GetSetting("debug", false);
			this.MaxPoolSize = this.GetSetting("maxPoolSize", -1);
		}

		public bool Enabled { get; private set; }
		public bool Debug { get; private set; }
		public int MaxPoolSize { get; private set; }
	}
}

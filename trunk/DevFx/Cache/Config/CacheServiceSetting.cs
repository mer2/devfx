/* Copyright(c) 2005-2011 R2@DevFx.NET, License(LGPL) */

using HTB.DevFx.Config;

[assembly: ConfigResource("res://HTB.DevFx.Cache.Config.htb.devfx.cache.config", Index = 500)]

namespace HTB.DevFx.Cache.Config
{
	internal class CacheServiceSetting : ConfigSettingElement, ICacheServiceSetting
	{
		protected override void OnConfigSettingChanged() {
			this.Caches = this.GetSettings<CacheSetting>(null).ToArray();
		}

		public ICacheSetting[] Caches { get; private set; }

		internal class CacheSetting : ConfigSettingElement, ICacheSetting
		{
			protected override void OnConfigSettingChanged() {
				this.Name = this.GetSetting("name");
				this.TypeName = this.GetSetting("type");
				this.Interval = this.GetSetting("interval", 1000D);
				this.CacheStorage = this.GetSetting<CacheStorageSetting>("cacheStorage");
			}

			public string Name { get; private set; }
			public string TypeName { get; private set; }
			public double Interval { get; private set; }
			public ICacheStorageSetting CacheStorage { get; private set; }
		}

		internal class CacheStorageSetting : ConfigSettingElement, ICacheStorageSetting
		{
			protected override void OnConfigSettingChanged() {
				this.TypeName = this.GetSetting("type");
			}

			public string TypeName { get; private set; }
		}
	}
}

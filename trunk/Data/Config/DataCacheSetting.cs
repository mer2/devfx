/* Copyright(c) 2005-2011 R2@DevFx.NET, License(LGPL) */

using System;
using HTB.DevFx.Config;
using HTB.DevFx.Data.Entities;

namespace HTB.DevFx.Data.Config
{
	internal class DataCacheSetting : ConfigSettingElement, IDataCacheSetting
	{
		protected override void OnConfigSettingChanged() {
			this.Cacheable = this.GetSetting("cacheable", true);
			this.CacheName = this.GetSetting("cacheName");
			this.CacheKey = this.GetSetting("cacheKey");
			this.CacheAction = this.GetSetting("cacheAction", CacheAction.Cache);
			this.Parameters = this.GetSetting("parameters");
			this.DependencyName = this.GetSetting("dependency");
		}

		public bool Cacheable { get; private set; }
		public string CacheName { get; private set; }
		public string CacheKey { get; private set; }
		public CacheAction CacheAction { get; private set; }
		public string Parameters { get; private set; }
		public string DependencyName { get; private set; }
	}
}

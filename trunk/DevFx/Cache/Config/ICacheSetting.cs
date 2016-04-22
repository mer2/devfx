/* Copyright(c) 2005-2011 R2@DevFx.NET, License(LGPL) */

using HTB.DevFx.Config;

namespace HTB.DevFx.Cache.Config
{
	public interface ICacheSetting : IConfigSettingRequired
	{
		string Name { get; }
		string TypeName { get; }
		double Interval { get; }
		ICacheStorageSetting CacheStorage { get; }
	}
}

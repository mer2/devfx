/* Copyright(c) 2005-2011 R2@DevFx.NET, License(LGPL) */

using HTB.DevFx.Data.Entities;

namespace HTB.DevFx.Data.Config
{
	public interface IDataCacheSetting
	{
		bool Cacheable { get; }
		string CacheName { get; }
		string CacheKey { get; }
		CacheAction CacheAction { get; }
		string Parameters { get; }
		string DependencyName { get; }
	}
}

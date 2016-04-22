/* Copyright(c) 2005-2011 R2@DevFx.NET, License(LGPL) */

using HTB.DevFx.Cache.Config;
using HTB.DevFx.Core;

namespace HTB.DevFx.Cache
{
	public interface ICacheInternal : ICache, IInitializable<ICacheSetting>
	{
		string CacheName { get; }
	}
}

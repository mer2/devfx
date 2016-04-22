/* Copyright(c) 2005-2011 R2@DevFx.NET, License(LGPL) */

using HTB.DevFx.Config;

namespace HTB.DevFx.Data.Config
{
	public interface IDataStorageSetting : IConfigSettingRequired
	{
		string Name { get; }
		string ConnectionString { get; }
		string StorageTypeName { get; }
	}
}

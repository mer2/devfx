/* Copyright(c) 2005-2011 R2@DevFx.NET, License(LGPL) */

namespace DevFx.Data.Settings
{
	public interface IDataStorageContextSetting
	{
		string DefaultStorageName { get; }
		IDataStorageSetting[] DataStorages { get; }
	}
}
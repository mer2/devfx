/* Copyright(c) 2005-2011 R2@DevFx.NET, License(LGPL) */

namespace DevFx.Data.Settings
{
	public interface IDataStorageSetting
	{
		string Name { get; }
		string ConnectionName { get; }
		string StorageTypeName { get; }
	}
}

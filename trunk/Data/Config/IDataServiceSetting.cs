/* Copyright(c) 2005-2011 R2@DevFx.NET, License(LGPL) */

namespace HTB.DevFx.Data.Config
{
	public interface IDataServiceSetting
	{
		bool Debug { get; }
		IResultHandlerFactoryContextSetting ResultHandlerFactoryContext { get; }
		IDataStorageContextSetting DataStorageContext { get; }
		IStatementContextSetting[] StatementContexts { get; }
	}
}

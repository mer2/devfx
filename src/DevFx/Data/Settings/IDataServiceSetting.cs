/* Copyright(c) 2005-2011 R2@DevFx.NET, License(LGPL) */

namespace DevFx.Data.Settings
{
	[Service]
	public interface IDataServiceSetting
	{
		bool Debug { get; }
		IResultHandlerFactoryContextSetting ResultHandlerFactoryContext { get; }
		IConnectionStringSetting[] ConnectionStrings { get; }
		IDataStorageContextSetting DataStorageContext { get; }
		IStatementContextSetting[] StatementContexts { get; }
	}
}
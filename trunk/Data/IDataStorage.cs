/* Copyright(c) 2005-2011 R2@DevFx.NET, License(LGPL) */

using System.Data.Common;

namespace HTB.DevFx.Data
{
	public interface IDataStorage
	{
		string ProviderName { get; }
		DbProviderFactory DataProviderFactory { get; }
		DbConnection GetConnection();
		DbCommand GetCommand(IDbExecuteContext ctx);
		void SetDbType(DbParameter dbParameter, string dbTypeName);
	}
}

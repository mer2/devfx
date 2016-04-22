/* Copyright(c) 2005-2011 R2@DevFx.NET, License(LGPL) */

using System;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;

namespace HTB.DevFx.Data
{
	public class SqlServerDataStorage : DataStorageBase
	{
		protected override void SetDbType(DbParameter dbParameter, string dbTypeName) {
			if (dbParameter is SqlParameter) {
				((SqlParameter)dbParameter).SqlDbType = (SqlDbType)Enum.Parse(typeof(SqlDbType), dbTypeName, true);
			} else {
				base.SetDbType(dbParameter, dbTypeName);
			}
		} 
	}
}

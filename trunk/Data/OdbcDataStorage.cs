/* Copyright(c) 2005-2011 R2@DevFx.NET, License(LGPL) */

using System;
using System.Data.Common;
using System.Data.Odbc;

namespace HTB.DevFx.Data
{
	public class OdbcDataStorage : DataStorageBase
	{
		protected override void SetDbType(DbParameter dbParameter, string dbTypeName) {
			if (dbParameter is OdbcParameter) {
				((OdbcParameter)dbParameter).OdbcType = (OdbcType)Enum.Parse(typeof(OdbcType), dbTypeName, true);
			} else {
				base.SetDbType(dbParameter, dbTypeName);
			}
		}
	}
}

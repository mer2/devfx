/* Copyright(c) 2005-2011 R2@DevFx.NET, License(LGPL) */

using System;
using System.Data.Common;
using System.Data.OleDb;

namespace HTB.DevFx.Data
{
	public class OleDbDataStorage : DataStorageBase
	{
		protected override void SetDbType(DbParameter dbParameter, string dbTypeName) {
			if (dbParameter is OleDbParameter) {
				((OleDbParameter)dbParameter).OleDbType = (OleDbType)Enum.Parse(typeof(OleDbType), dbTypeName, true);
			} else {
				base.SetDbType(dbParameter, dbTypeName);
			}
		}
	}
}

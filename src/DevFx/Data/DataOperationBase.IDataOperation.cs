/* Copyright(c) 2005-2011 R2@DevFx.NET, License(LGPL) */

using System;
using System.Data;

namespace DevFx.Data
{
	partial class DataOperationBase : IDataOperation
	{
		#region IDataOperation Members

		object IDataOperation.Execute(string statementName, object parameters, Type resultType) {
			return this.Execute(statementName, parameters, resultType);
		}

		T IDataOperation.Execute<T>(string statementName, object parameters) {
			return this.Execute<T>(statementName, parameters);
		}

		object IDataOperation.ExecuteSql(string sql, object parameters, string storageName, Type resultType, CommandType sqlType) {
			return this.ExecuteSql(sql, parameters, storageName, resultType, sqlType);
		}

		T IDataOperation.ExecuteSql<T>(string sql, object parameters, string storageName, CommandType sqlType) {
			return this.ExecuteSql<T>(sql, parameters, storageName, sqlType);
		}

		#endregion
	}
}

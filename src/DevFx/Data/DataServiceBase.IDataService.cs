/* Copyright(c) 2005-2011 R2@DevFx.NET, License(LGPL) */

using System;
using System.Data;

namespace DevFx.Data
{
	partial class DataServiceBase : IDataService
	{
		#region IDataOperation Members

		object IDataOperation.Execute(string statementName, object parameters, Type resultType) {
			return this.DefaultDataSession.Execute(statementName, parameters, resultType);
		}

		T IDataOperation.Execute<T>(string statementName, object parameters) {
			return this.DefaultDataSession.Execute<T>(statementName, parameters);
		}

		object IDataOperation.ExecuteSql(string sql, object parameters, string storageName, Type resultType, CommandType sqlType) {
			return this.DefaultDataSession.ExecuteSql(sql, parameters, storageName, resultType, sqlType);
		}

		T IDataOperation.ExecuteSql<T>(string sql, object parameters, string storageName, CommandType sqlType) {
			return this.DefaultDataSession.ExecuteSql<T>(sql, parameters, storageName, sqlType);
		}

		#endregion

		#region IDataService Members

		IDataSession IDataService.BeginSession() {
			return this.BeginSessionInternal(null, false);
		}

		IDataSession IDataService.BeginSession(string storageName) {
			return this.BeginSessionInternal(storageName, false);
		}

		IDataSession IDataService.BeginSession(string storageName, bool beginTransaction) {
			return this.BeginSessionInternal(storageName, beginTransaction);
		}

		#endregion
	}
}

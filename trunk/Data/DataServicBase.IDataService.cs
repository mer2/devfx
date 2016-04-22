/* Copyright(c) 2005-2011 R2@DevFx.NET, License(LGPL) */

namespace HTB.DevFx.Data
{
	partial class DataServiceBase : IDataService
	{
		#region IDataOperation Members

		object IDataOperation.Execute(string statementName, object parameters) {
			return this.DefaultDataSession.Execute(statementName, parameters);
		}

		T IDataOperation.Execute<T>(string statementName, object parameters) {
			return this.DefaultDataSession.Execute<T>(statementName, parameters);
		}

		object IDataOperation.Execute(string statementName, object parameters, object result) {
			return this.DefaultDataSession.Execute(statementName, parameters, result);
		}

		T IDataOperation.Execute<T>(string statementName, object parameters, T result) {
			return this.DefaultDataSession.Execute(statementName, parameters, result);
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

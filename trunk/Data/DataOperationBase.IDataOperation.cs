/* Copyright(c) 2005-2011 R2@DevFx.NET, License(LGPL) */

namespace HTB.DevFx.Data
{
	partial class DataOperationBase : IDataOperation
	{
		#region IDataOperation Members

		object IDataOperation.Execute(string statementName, object parameters) {
			return this.Execute(statementName, parameters);
		}

		T IDataOperation.Execute<T>(string statementName, object parameters) {
			return this.Execute<T>(statementName, parameters);
		}

		object IDataOperation.Execute(string statementName, object parameters, object result) {
			return this.Execute(statementName, parameters, result);
		}

		T IDataOperation.Execute<T>(string statementName, object parameters, T result) {
			return this.Execute(statementName, parameters, result);
		}

		#endregion
	}
}

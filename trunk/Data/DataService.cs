/* Copyright(c) 2005-2011 R2@DevFx.NET, License(LGPL) */

namespace HTB.DevFx.Data
{
	public class DataService : DataServiceBase
	{
		protected DataService() { }

		#region Static Members

		public static IDataService Current {
			get { return DevFx.ObjectService.GetObject<IDataService>(); }
		}

		public static object Execute(string statementName, object parameters) {
			return Current.Execute(statementName, parameters);
		}

		public static T Execute<T>(string statementName, object parameters) {
			return Current.Execute<T>(statementName, parameters);
		}

		public static object Execute(string statementName, object parameters, object result) {
			return Current.Execute(statementName, parameters, result);
		}

		public static T Execute<T>(string statementName, object parameters, T result) {
			return Current.Execute(statementName, parameters, result);
		}

		public static IDataSession BeginSession() {
			return Current.BeginSession();
		}
		
		public static IDataSession BeginSession(string storageName) {
			return Current.BeginSession(storageName);
		}
		
		public static IDataSession BeginSession(string storageName, bool beginTransation) {
			return Current.BeginSession(storageName, beginTransation);
		}

		#endregion
	}
}

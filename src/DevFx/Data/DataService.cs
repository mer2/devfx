/* Copyright(c) 2005-2011 R2@DevFx.NET, License(LGPL) */

using System;
using System.Data;

namespace DevFx.Data
{
	[Object]
	public class DataService : DataServiceBase
	{
		protected DataService() { }

		#region Static Members

		private static IDataService current;
		public static IDataService Current => current ?? (current = DevFx.ObjectService.GetObject<IDataService>());

		public static object Execute(string statementName, object parameters) {
			return Current.Execute(statementName, parameters);
		}

		public static T Execute<T>(string statementName, object parameters) {
			return Current.Execute<T>(statementName, parameters);
		}

		public static object ExecuteSql(string sql, object parameters, string storageName = null, CommandType sqlType = CommandType.Text) {
			return Current.ExecuteSql(sql, parameters, storageName, null, sqlType);
		}

		public static T ExecuteSql<T>(string sql, object parameters, string storageName = null, CommandType sqlType = CommandType.Text) {
			return Current.ExecuteSql<T>(sql, parameters, storageName, sqlType);
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

		#region DataExecutor

		public static void Execute(bool beginTransation, Action<IDataExecuteContext> action) {
			using (var ds = BeginSession(null, beginTransation)) {
				using (var helper = new DataExecuteContext(ds, beginTransation)) {
					action(helper);
				}
			}
		}

		public static T Execute<T>(bool beginTransation, Func<IDataExecuteContext, T> func) {
			using (var ds = BeginSession(null, beginTransation)) {
				using (var helper = new DataExecuteContext(ds, beginTransation)) {
					return func(helper);
				}
			}
		}

		public static void Execute<T, T1>(bool beginTransation, Func<T, T1, bool> func)
			where T : class, ISessionDataService
			where T1 : class, ISessionDataService {
			using (var ds = BeginSession(null, beginTransation)) {
				using (var helper = new DataExecuteContext(ds, beginTransation)) {
					var dt = helper.GetDataService<T>();
					var dt1 = helper.GetDataService<T1>();
					if (func(dt, dt1) && beginTransation) {
						helper.Commit();
					}
				}
			}
		}

		public static void Execute<T>(bool beginTransation, Func<T, bool> func) where T : class, ISessionDataService {
			using (var ds = BeginSession(null, beginTransation)) {
				using (var helper = new DataExecuteContext(ds, beginTransation)) {
					var dt = helper.GetDataService<T>();
					if (func(dt) && beginTransation) {
						helper.Commit();
					}
				}
			}
		}

		#endregion
	}
}

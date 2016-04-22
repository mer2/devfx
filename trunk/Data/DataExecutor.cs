using System;

namespace HTB.DevFx.Data
{
	public abstract class DataExecutor : DataExecutorBase
	{
		public static void Execute(bool beginTransation, Action<IDataExecuteContext> action) {
			using(var ds = DataService.BeginSession(null, beginTransation)) {
				using(var helper = new DataExecuteContext(ds, beginTransation)) {
					action(helper);
				}
			}
		}

		public static T Execute<T>(bool beginTransation, System.Func<IDataExecuteContext, T> func) {
			using(var ds = DataService.BeginSession(null, beginTransation)) {
				using(var helper = new DataExecuteContext(ds, beginTransation)) {
					return func(helper);
				}
			}
		}

		public static void Execute<T, T1>(bool beginTransation, System.Func<T, T1, bool> func)
			where T : ISessionDataService
			where T1 : ISessionDataService {
			using(var ds = DataService.BeginSession(null, beginTransation)) {
				using(var helper = new DataExecuteContext(ds, beginTransation)) {
					var dt = helper.GetDataService<T>();
					var dt1 = helper.GetDataService<T1>();
					if(func(dt, dt1) && beginTransation) {
						helper.Commit();
					}
				}
			}
		}

		public static void Execute<T>(bool beginTransation, System.Func<T, bool> func) where T : ISessionDataService {
			using(var ds = DataService.BeginSession(null, beginTransation)) {
				using(var helper = new DataExecuteContext(ds, beginTransation)) {
					var dt = helper.GetDataService<T>();
					if(func(dt) && beginTransation) {
						helper.Commit();
					}
				}
			}
		}
	}
}

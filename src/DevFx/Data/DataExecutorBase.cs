using System;
using System.Collections.Generic;

namespace DevFx.Data
{
	public abstract class DataExecutorBase : ISessionDataService
	{
		protected virtual IDataSession CurrentSession { get; set; }
		protected virtual bool InTransaction { get; set; }
		protected virtual IDataOperation GetDataOperation() {
			return (IDataOperation)this.CurrentSession ?? DataService.Current;
		}

		void ISessionDataService.SetDataSession(IDataSession session, bool inTransation) {
			this.CurrentSession = session;
			if(session != null) {
				this.InTransaction = inTransation;
			}
		}

		protected virtual void Dispose() { }
		void IDisposable.Dispose() {
			this.Dispose();
		}

		protected virtual void BeginTransation(Func<IDataOperation, bool> func) {
			if(!this.InTransaction) {
				using(var ds = DataService.BeginSession(null, true)) {
					if(func(ds)) {
						ds.CommitTransaction();
					}
				}
			} else {
				var dop = this.GetDataOperation();
				func(dop);
			}
		}

		protected virtual void BeginSession(Action<IDataOperation> action) {
			var dop = this.GetDataOperation();
			action(dop);
		}

		protected virtual object Execute(string statementName, object parameters) {
			var dop = this.GetDataOperation();
			return dop.Execute(statementName, parameters);
		}

		protected virtual T Execute<T>(string statementName, object parameters) {
			var dop = this.GetDataOperation();
			return dop.Execute<T>(statementName, parameters);
		}
	}

	public class DataExecuteContext : IDataExecuteContext, IDisposable
	{
		public DataExecuteContext(IDataSession dataSessio, bool beginTransation) {
			this.dataSession = dataSessio;
			this.beginTransation = beginTransation;
		}
		private readonly IDataSession dataSession;
		private readonly bool beginTransation;
		private readonly Dictionary<Type, ISessionDataService> services = new Dictionary<Type, ISessionDataService>();

		public T GetDataService<T>() where T : class, ISessionDataService {
			var type = typeof(T);
			if(services.ContainsKey(type)) {
				return (T)services[type];
			}
			var dt = ObjectService.Current.CreateObject<T>();
			dt.SetDataSession(this.dataSession, this.beginTransation);
			services.Add(type, dt);
			return dt;
		}

		public IDataSession GetDataSession() {
			return this.dataSession;
		}

		public void Commit() {
			if(this.beginTransation) {
				this.dataSession.CommitTransaction();
			}
		}

		public void Rollback() {
			if(this.beginTransation) {
				this.dataSession.RollbackTransaction();
			}
		}

		void IDisposable.Dispose() {
			foreach(var ds in this.services.Values) {
				ds.Dispose();
			}
		}
	}
}

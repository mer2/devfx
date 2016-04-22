/* Copyright(c) 2005-2011 R2@DevFx.NET, License(LGPL) */

using System.Collections;
using System.Data.Common;
using HTB.DevFx.Data.Config;
using HTB.DevFx.Log;

namespace HTB.DevFx.Data
{
	public abstract partial class DataSessionBase : DataOperationBase
	{
		protected DataSessionBase(DataServiceBase dataService, string storageName, bool beginTransaction) : base(dataService.ResultHandlerFactory) {
			this.dataService = dataService;
			this.IsInTransaction = beginTransaction;
			if (!string.IsNullOrEmpty(storageName)) {
				this.DataStorage = dataService.GetDataStorage(storageName);
			}
		}

		private readonly DataServiceBase dataService;
		protected virtual DataServiceBase DataService {
			get { return this.dataService; }
		}

		private readonly object lockTransaction = new object();
		private volatile DbTransaction transaction;
		protected bool IsInTransaction { get; set; }
		protected DbTransaction Transaction {
			get { return this.transaction; }
			set { this.transaction = value; }
		}

		private readonly object lockConnection = new object();
		private volatile DbConnection connection;
		protected IDataStorage DataStorage { get; set; }
		protected DbConnection Connection {
			get { return this.connection; }
			set { this.connection = value; }
		}

		protected virtual DbConnection GetConnection(IStatementSetting statement) {
			if(this.connection != null) {
				return this.connection;
			}
			var storage = this.DataStorage ?? this.DataService.GetDataStorage(statement);
			if (this.connection == null) {
				lock(this.lockConnection) {
					if(this.connection == null) {
						this.connection = storage.GetConnection();
					}
				}
			}
			return this.connection;
		}

		protected internal virtual IDataStorage GetDataStorage(IStatementSetting statement) {
			return this.DataStorage ?? this.DataService.GetDataStorage(statement);
		}

		protected internal virtual IDbCommandWrap GetCommandWrap(IStatementSetting statement) {
			DbConnection connection;
			var closeConnectionOnCommandDisposed = true;
			if (this.IsInTransaction) {
				if (this.transaction == null) {
					lock (this.lockTransaction) {
						if (this.transaction == null) {
							connection = this.GetConnection(statement);
							connection.Open();
							if(this.DataService.Debug) {
								LogService.WriteLog(this, LogLevel.DEBUG, "Connection open with transaction:" + connection.GetHashCode());
							}
							this.transaction = connection.BeginTransaction();
						}
					}
				}
				connection = this.Transaction.Connection;
				closeConnectionOnCommandDisposed = false;
			} else {
				connection = this.GetConnection(statement);
				connection.Open();
				if(this.DataService.Debug) {
					LogService.WriteLog(this, LogLevel.DEBUG, "Connection open without transaction:" + connection.GetHashCode());
				}
			}
			var command = connection.CreateCommand();
			command.Transaction = this.Transaction;
			return new DbCommandWrap(command, closeConnectionOnCommandDisposed);
		}

		protected override IDbExecuteContext GetExecuteContext(string statementName, IDictionary parameters) {
			var statement = this.DataService.GetStatementSetting(statementName, true);
			return new DbExecuteContext(this.DataService, statement, this, parameters, null);
		}

		protected virtual void Dispose(bool disposing) {
			if (disposing) {
				if (this.transaction != null) {
					this.transaction.Dispose();
				}
				this.transaction = null;
				if(this.connection != null) {
					this.connection.Close();
					if(this.DataService.Debug) {
						LogService.WriteLog(this, LogLevel.DEBUG, "Connection {0} closed, transaction is {1}", connection.GetHashCode(), this.IsInTransaction);
					}
				}
				this.connection = null;
			}
		}

		#region Transaction

		protected virtual void CommitTransaction() {
			if (this.transaction != null) {
				this.Transaction.Commit();
			}
		}

		protected virtual void RollbackTransaction() {
			if (this.transaction != null) {
				this.Transaction.Rollback();
			}
		}

		#endregion

		#region DataSession
		
		internal class DataSession : DataSessionBase
		{
			public DataSession(DataServiceBase dataService, string storageName, bool isInTransaction) : base(dataService, storageName, isInTransaction) { }
		}

		#endregion
	}
}

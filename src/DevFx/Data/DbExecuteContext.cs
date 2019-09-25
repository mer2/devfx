/* Copyright(c) 2005-2011 R2@DevFx.NET, License(LGPL) */

using System;
using System.Collections;
using DevFx.Core;
using DevFx.Data.Settings;

namespace DevFx.Data
{
	internal class DbExecuteContext : ObjectContextBase, IDbExecuteContext
	{
		public DbExecuteContext(IDataService dataService, IStatementSetting statement, IDataStorage dataStorage, IDbCommandWrap commandWrap, IDictionary parameters, IDictionary items) : base(items) {
			this.DataService = dataService;
			this.Statement = statement;
			this.DataStorage = dataStorage;
			this.CommandWrap = commandWrap;
			this.Parameters = parameters;
		}

		public DbExecuteContext(IDataService dataService, IStatementSetting statement, DataSessionBase dataSession, IDictionary parameters, IDictionary items) : this(dataService, statement, null, null, parameters, items) {
			this.dataSession = dataSession;
		}

		public IDataService DataService { get; }
		public IStatementSetting Statement { get; }
		public IDictionary Parameters { get; }

		private readonly DataSessionBase dataSession;
		private IDataStorage dataStorage;
		public IDataStorage DataStorage {
			get => this.dataStorage ?? (this.dataStorage = this.dataSession.GetDataStorage(this.Statement));
			private set => this.dataStorage = value;
		}

		private IDbCommandWrap commandWrap;
		public IDbCommandWrap CommandWrap {
			get => this.commandWrap ?? (this.commandWrap = this.dataSession.GetCommandWrap(this.Statement));
			private set => this.commandWrap = value;
		}

		#region Implementation of IDisposable

		protected void Dispose(bool disposing) {
			if(disposing) {
				var wrap = this.commandWrap;
				if(wrap != null) {
					wrap.Dispose();
				}
			}
		}

		public void Dispose() {
			this.Dispose(true);
			GC.SuppressFinalize(this);
		}

		~DbExecuteContext() {
			this.Dispose(true);
		}

		#endregion
	}
}

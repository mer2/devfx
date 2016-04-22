/* Copyright(c) 2005-2011 R2@DevFx.NET, License(LGPL) */

using System.Collections.Generic;
using HTB.DevFx.Core;
using HTB.DevFx.Data.Config;

namespace HTB.DevFx.Data
{
	public abstract partial class DataServiceBase : ServiceBase<IDataServiceSetting>
	{
		protected override void OnInit() {
			this.Debug = this.Setting.Debug;
			this.InitResultHandlerFactory();
			this.InitDataStorages();
			this.InitDataStatements();
		}

		private void InitResultHandlerFactory() {
			var typeName = this.Setting.ResultHandlerFactoryContext.FactoryTypeName;
			var factory = this.ObjectService.GetOrCreateObject<IResultHandlerFactory>(typeName);
			var initializable = factory as IInitializable<IResultHandlerFactoryContextSetting>;
			if (initializable != null) {
				initializable.Init(this.Setting.ResultHandlerFactoryContext);
			}
			this.ResultHandlerFactory = factory;
		}

		private void InitDataStorages() {
			var dict = this.storages = new Dictionary<string, IDataStorage>();
			if(this.Setting.DataStorageContext != null && this.Setting.DataStorageContext.DataStorages != null) {
				foreach(var storage in this.Setting.DataStorageContext.DataStorages) {
					var typeName = storage.StorageTypeName;
					IDataStorage dataStorage = null;
					if (!string.IsNullOrEmpty(typeName)) {
						dataStorage = this.ObjectService.GetOrCreateObject<IDataStorage>(typeName);
					}
					if(dataStorage == null) {
						dataStorage = new DefaultDataStorage();
					}
					var initializable = dataStorage as IInitializable<IDataStorageSetting>;
					if (initializable != null) {
						initializable.Init(storage);
					}
					dict.Add(storage.Name, dataStorage);
				}
			}
		}

		private void InitDataStatements() {
			var dict = this.statements = new Dictionary<string, IStatementSetting>();
			if(this.Setting.StatementContexts != null && this.Setting.StatementContexts.Length > 0) {
				foreach(var contextSetting in this.Setting.StatementContexts) {
					var spaceName = contextSetting.Name;
					if(!string.IsNullOrEmpty(spaceName)) {
						spaceName += ".";
					}
					foreach(var statement in contextSetting.Statements) {
						dict.Add(spaceName + statement.Name, statement);
					}
				}
			}
		}

		private Dictionary<string, IStatementSetting> statements;
		private Dictionary<string, IDataStorage> storages;

		protected virtual IDataSession DefaultDataSession {
			get { return new DataSessionBase.DataSession(this, null, false); }
		}
		protected internal bool Debug { get; private set; }
		protected internal IResultHandlerFactory ResultHandlerFactory { get; private set; }

		protected internal virtual IStatementSetting GetStatementSetting(string statementName, bool throwOnNull) {
			IStatementSetting statementSetting;
			this.statements.TryGetValue(statementName, out statementSetting);
			if(statementSetting == null && throwOnNull) {
				throw new DataException("指定的StatementName不存在：" + statementName);
			}
			return statementSetting;
		}

		protected internal virtual IDataStorage GetDataStorage(string storageName) {
			IDataStorage storage;
			this.storages.TryGetValue(storageName, out storage);
			return storage;
		}

		protected internal virtual IDataStorage GetDataStorage(IStatementSetting statementSetting) {
			return this.GetDataStorage(statementSetting.DataStorageName);
		}

		protected virtual DataSessionBase BeginSessionInternal(string storageName, bool beginTransaction) {
			return new DataSessionBase.DataSession(this, storageName, beginTransaction);
		}
	}
}

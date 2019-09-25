/* Copyright(c) 2005-2011 R2@DevFx.NET, License(LGPL) */

using DevFx.Core;
using DevFx.Data.Settings;
using DevFx.Utils;
using System;
using System.Collections.Generic;
using System.Data;

namespace DevFx.Data
{
	public abstract partial class DataServiceBase : IInitializable<IDataServiceSetting>
	{
		[Autowired(Required = true)]
		protected IObjectService ObjectService { get; set; }
		protected IDataServiceSetting Setting { get; set; }
		public void Init(IDataServiceSetting setting) {
			this.Setting = setting;

			this.InitConnectionStrings();
			this.InitResultHandlerFactory();
			this.InitDataStorages();
			this.InitDataStatements();
		}

		private void InitConnectionStrings() {
			var dict = this.connectionStrings = new Dictionary<string, IConnectionStringSetting>();
			if(this.Setting?.ConnectionStrings != null) {
				foreach(var connectionString in this.Setting.ConnectionStrings) {
					dict.Add(connectionString.Name, connectionString);
				}
			}
		}

		private void InitResultHandlerFactory() {
			var typeName = this.Setting.ResultHandlerFactoryContext.FactoryTypeName;
			var factory = this.ObjectService.GetOrCreateObject<IResultHandlerFactory>(typeName);
			factory.Init(this.ObjectService, this.Setting.ResultHandlerFactoryContext);
			this.ResultHandlerFactory = factory;
		}

		private void InitDataStorages() {
			var dict = this.storages = new Dictionary<string, IDataStorage>();
			if(this.Setting.DataStorageContext?.DataStorages != null) {
				foreach(var storage in this.Setting.DataStorageContext.DataStorages) {
					var typeName = storage.StorageTypeName;
					IDataStorage dataStorage = null;
					if (!string.IsNullOrEmpty(typeName)) {
						dataStorage = this.ObjectService.GetOrCreateObject<IDataStorage>(typeName);
					}
					if(dataStorage == null) {
						throw new DataException($"未找到名为 {typeName} 的数据库驱动");
					}
					this.connectionStrings.TryGetValue(storage.ConnectionName, out var setting);
					if (setting == null) {
						throw new DataException($"未找到名为 {storage.ConnectionName} 的数据库字符串配置");
					}
					dataStorage.Init(this.ObjectService, storage, setting);
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
					var statements = contextSetting.Statements;
					if(statements != null && statements.Length > 0) {
						foreach (var statement in statements) {
							dict[spaceName + statement.Name] = statement;//替换代码定义的Statement
						}
					}
				}
			}
		}

		private Dictionary<string, IConnectionStringSetting> connectionStrings;
		private Dictionary<string, IStatementSetting> statements;
		private Dictionary<string, IDataStorage> storages;

		protected internal bool Debug => this.Setting.Debug;
		protected virtual IDataSession DefaultDataSession => new DataSessionBase.DataSession(this, null, false);
		protected internal IResultHandlerFactory ResultHandlerFactory { get; private set; }

		protected internal virtual IStatementSetting GetStatementSetting(string statementName, bool throwOnNull) {
			if (!this.statements.TryGetValue(statementName, out var statementSetting) && throwOnNull) {
				throw new DataException("指定的StatementName不存在：" + statementName);
			}
			return statementSetting;
		}

		//获取仅由Sql语句组成的Sql状态配置
		protected internal virtual IStatementSetting GetSqlStatementSetting(string sql, CommandType sqlType, string storageName) {
			if (string.IsNullOrEmpty(storageName)) {
				storageName = this.Setting.DataStorageContext.DefaultStorageName;
			}
			var statementName = HashHelper.Hash($"{storageName}:{sql}", "md5");
			if (!this.statements.TryGetValue(statementName, out var statementSetting)) {
				lock (statementName) {
					if (!this.statements.TryGetValue(statementName, out statementSetting)) {
						statementSetting = new SqlStatementSetting(statementName, sql, storageName) { CommandType = sqlType };
						this.statements.Add(statementName, statementSetting);
					}
				}
			}
			return statementSetting;
		}

		internal virtual void AddSqlStatmentSetting(SqlStatementSetting statementSetting, bool replaceExists) {
			if (this.statements.ContainsKey(statementSetting.Name) && !replaceExists) {
				return;
			}
			if(string.IsNullOrEmpty(statementSetting.DataStorageName)) {
				statementSetting.DataStorageName = this.Setting.DataStorageContext.DefaultStorageName;
			}
			this.statements[statementSetting.Name] = statementSetting;
		}

		protected internal virtual IDataStorage GetDataStorage(string storageName) {
			if(string.IsNullOrEmpty(storageName)) {
				throw new ArgumentNullException(nameof(storageName));
			}
			this.storages.TryGetValue(storageName, out var storage);
			return storage;
		}

		protected internal virtual IDataStorage GetDataStorage(IStatementSetting statementSetting) {
			var storageName = statementSetting.DataStorageName;
			if (string.IsNullOrEmpty(storageName)) {
				throw new DataException($"未找到名为 [{statementSetting.Name}] 的Sql语句对应的存储名，未配置Sql存储？");
			}
			return this.GetDataStorage(statementSetting.DataStorageName);
		}

		protected virtual DataSessionBase BeginSessionInternal(string storageName, bool beginTransaction) {
			return new DataSessionBase.DataSession(this, storageName, beginTransaction);
		}
	}
}

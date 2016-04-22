/* Copyright(c) 2005-2011 R2@DevFx.NET, License(LGPL) */

using System;
using System.Data;
using System.Data.Common;
using System.Configuration;
using HTB.DevFx.Core;
using HTB.DevFx.Data.Config;

namespace HTB.DevFx.Data
{
	public abstract class DataStorageBase : IDataStorage, IInitializable<IDataStorageSetting>
	{
		protected IObjectService ObjectService { get; private set; }
		protected IDataStorageSetting Setting { get; private set; }
		protected string ConnectionStringName { get; private set; }
		protected string ConnectionString { get; set; }

		protected virtual void Init(IDataStorageSetting setting) {
			this.ObjectService = DevFx.ObjectService.Current;
			this.Setting = setting;
			this.ConnectionStringName = setting.ConnectionString;

			var connectionSetting = ConfigurationManager.ConnectionStrings[this.ConnectionStringName];
			if (connectionSetting == null) {
				throw new DataException(string.Format("未找到名为 {0} 的数据库字符串配置", this.ConnectionStringName));
			}
			var providerName = connectionSetting.ProviderName;
			if (string.IsNullOrEmpty(providerName)) {
				providerName = "System.Data.SqlClient";
			}
			this.ConnectionString = connectionSetting.ConnectionString;
			this.DataProviderFactory = DbProviderFactories.GetFactory(providerName);
			this.ProviderName = providerName;
			this.CheckDataProviderFactory();
		}

		protected virtual void CheckDataProviderFactory() {
		}

		protected virtual IDbCommandBuilder GetCommandBuilder(IDbExecuteContext ctx) {
			var typeName = ctx.Statement.CommandBuilderTypeName;
			return string.IsNullOrEmpty(typeName) ? this.ObjectService.GetObject<IDbCommandBuilder>() : this.ObjectService.GetObject<IDbCommandBuilder>(typeName);
		}

		public string ProviderName { get; private set; }
		public DbProviderFactory DataProviderFactory { get; private set; }

		public virtual DbConnection GetConnection() {
			var conn = this.DataProviderFactory.CreateConnection();
			conn.ConnectionString = this.ConnectionString;
			return conn;
		}

		public virtual DbCommand GetCommand(IDbExecuteContext ctx) {
			var builder = this.GetCommandBuilder(ctx);
			return builder.GetCommand(ctx);
		}

		void IDataStorage.SetDbType(DbParameter dbParameter, string dbTypeName) {
			this.SetDbType(dbParameter, dbTypeName);
		}

		protected virtual void SetDbType(DbParameter dbParameter, string dbTypeName) {
			dbParameter.DbType = (DbType)Enum.Parse(typeof(DbType), dbTypeName, true);
		}

		#region IInitializable<IDataStorageSetting> Members

		void IInitializable<IDataStorageSetting>.Init(IDataStorageSetting setting) {
			this.Init(setting);
		}

		#endregion
	}
}

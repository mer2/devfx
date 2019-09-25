/* Copyright(c) 2005-2011 R2@DevFx.NET, License(LGPL) */

using DevFx.Core;
using DevFx.Data.Settings;
using System;
using System.Data;
using System.Data.Common;

namespace DevFx.Data
{
	/// <inheritdoc />
	public abstract class DataStorageBase : IDataStorage
	{
		protected IObjectService ObjectService { get; private set; }
		protected IDataStorageSetting Setting { get; private set; }
		protected IConnectionStringSetting ConnectionStringSetting { get; private set; }
		protected abstract DbProviderFactory GetDbProviderFactory(IConnectionStringSetting connectionString);

		public virtual void Init(IObjectService objectService, IDataStorageSetting setting, IConnectionStringSetting connectionStringSetting) {
			this.ObjectService = objectService;
			this.Setting = setting;
			this.ConnectionStringSetting = connectionStringSetting;
			this.DataProviderFactory = this.GetDbProviderFactory(this.ConnectionStringSetting);
			this.CheckDataProviderFactory();
		}

		protected virtual void CheckDataProviderFactory() {
		}

		protected virtual IDbCommandBuilder GetCommandBuilder(IDbExecuteContext ctx) {
			var typeName = ctx.Statement.CommandBuilderTypeName;
			return string.IsNullOrEmpty(typeName) ? this.ObjectService.GetObject<IDbCommandBuilder>() : this.ObjectService.GetObject<IDbCommandBuilder>(typeName);
		}

		public string ProviderName => this.ConnectionStringSetting.ProviderName;
		public virtual DbProviderFactory DataProviderFactory { get; private set; }

		public virtual DbConnection GetConnection() {
			var conn = this.DataProviderFactory.CreateConnection();
			conn.ConnectionString = this.ConnectionStringSetting.ConnectionString;
			return conn;
		}

		public virtual DbCommand GetCommand(IDbExecuteContext ctx) {
			var builder = this.GetCommandBuilder(ctx);
			return builder.GetCommand(ctx);
		}

		void IDataStorage.SetDbType(DbParameter dbParameter, string dbTypeName) {
			this.SetDbType(dbParameter, dbTypeName);
		}

		public virtual void SetDbTypeFromValue(DbParameter dbParameter, object parameterValue) {
			if(parameterValue == null || parameterValue == DBNull.Value) {
				return;
			}
			if(parameterValue is string) {
				dbParameter.DbType = DbType.String;
			} else if(parameterValue is int) {
				dbParameter.DbType = DbType.Int32;
			} else if(parameterValue is long) {
				dbParameter.DbType = DbType.Int64;
			} else if(parameterValue is bool) {
				dbParameter.DbType = DbType.Boolean;
			} else if(parameterValue is DateTime) {
				dbParameter.DbType = DbType.DateTime2;
			} else if(parameterValue is float) {
				dbParameter.DbType = DbType.Single;
			} else if(parameterValue is double) {
				dbParameter.DbType = DbType.Double;
			} else if(parameterValue is decimal) {
				dbParameter.DbType = DbType.Decimal;
			} else if(parameterValue is byte) {
				dbParameter.DbType = DbType.Byte;
			} else if(parameterValue is short) {
				dbParameter.DbType = DbType.Int16;
			} else if(parameterValue is sbyte) {
				dbParameter.DbType = DbType.SByte;
			} else if(parameterValue is uint) {
				dbParameter.DbType = DbType.UInt32;
			} else if(parameterValue is ulong) {
				dbParameter.DbType = DbType.UInt64;
			} else if(parameterValue is ushort) {
				dbParameter.DbType = DbType.UInt16;
			}
		}

		protected virtual void SetDbType(DbParameter dbParameter, string dbTypeName) {
			dbParameter.DbType = (DbType)Enum.Parse(typeof(DbType), dbTypeName, true);
		}
	}
}

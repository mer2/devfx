using System;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using DevFx.Data.Settings;

namespace DevFx.Data.DataProviders
{
    public class SqlServerDataStorage : DataStorageBase
	{
		protected override DbProviderFactory GetDbProviderFactory(IConnectionStringSetting connectionString) {
			return SqlClientFactory.Instance;
		}

		protected override void SetDbType(DbParameter dbParameter, string dbTypeName) {
			if(dbParameter is SqlParameter parameter) {
				parameter.SqlDbType = (SqlDbType)Enum.Parse(typeof(SqlDbType), dbTypeName, true);
			} else {
				base.SetDbType(dbParameter, dbTypeName);
			}
		}

		/*public override void SetDbTypeFromValue(DbParameter dbParameter, object parameterValue) {
			if (!(dbParameter is SqlParameter parameter)) {
				base.SetDbTypeFromValue(dbParameter, parameterValue);
				return;
			}
			if(parameterValue is string) {
				parameter.SqlDbType = SqlDbType.NVarChar;
			} else if(parameterValue is int) {
				parameter.SqlDbType = SqlDbType.Int;
			} else if(parameterValue is long) {
				parameter.SqlDbType = SqlDbType.BigInt;
			} else if(parameterValue is bool) {
				parameter.SqlDbType = SqlDbType.Bit;
			} else if(parameterValue is DateTime) {
				parameter.SqlDbType = SqlDbType.DateTime2;
			} else if(parameterValue is float) {
				parameter.SqlDbType = SqlDbType.Float;
			} else if(parameterValue is double) {
				parameter.SqlDbType = SqlDbType.Float;
			} else if(parameterValue is decimal) {
				parameter.SqlDbType = SqlDbType.Decimal;
			} else if(parameterValue is byte) {
				parameter.SqlDbType = SqlDbType.TinyInt;
			} else if(parameterValue is short) {
				parameter.SqlDbType = SqlDbType.SmallInt;
			} else if(parameterValue is sbyte) {
				parameter.SqlDbType = SqlDbType.TinyInt;
			} else if(parameterValue is uint) {
				parameter.SqlDbType = SqlDbType.Int;
			} else if(parameterValue is ulong) {
				parameter.SqlDbType = SqlDbType.BigInt;
			} else if(parameterValue is ushort) {
				parameter.SqlDbType = SqlDbType.SmallInt;
			}
		}*/
	}
}

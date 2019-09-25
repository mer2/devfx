/* Copyright(c) 2005-2011 R2@DevFx.NET, License(LGPL) */

using DevFx.Core;
using DevFx.Data.Settings;
using System.Data.Common;

namespace DevFx.Data
{
	public interface IDataStorage
	{
		string ProviderName { get; }
		DbProviderFactory DataProviderFactory { get; }
		DbConnection GetConnection();
		DbCommand GetCommand(IDbExecuteContext ctx);
		/// <summary>
		/// 根据类型名称设置参数类型，比如dbType="varchar"
		/// </summary>
		/// <param name="dbParameter">SQL参数</param>
		/// <param name="dbTypeName">参数类型名称</param>
		void SetDbType(DbParameter dbParameter, string dbTypeName);
		/// <summary>
		/// 根据参数值的类型推断数据库对应的类型
		/// </summary>
		/// <param name="dbParameter">SQL参数</param>
		/// <param name="parameterValue">参数值</param>
		void SetDbTypeFromValue(DbParameter dbParameter, object parameterValue);

		void Init(IObjectService objectService, IDataStorageSetting setting, IConnectionStringSetting connectionStringSetting);
	}
}
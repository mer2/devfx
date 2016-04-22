/* Copyright(c) 2005-2013 R2@DevFx.NET, License(LGPL) */

using System;
using System.Collections;
using System.Data;
using System.Data.Common;
using HTB.DevFx.Data.Config;
using HTB.DevFx.Log;
using HTB.DevFx.Utils;

namespace HTB.DevFx.Data
{
	public class DbCommandBuilder : IDbCommandBuilder//从DataStorageBase拆分出来的
	{
		public virtual DbCommand GetCommand(IDbExecuteContext ctx) {
			var wrap = ctx.CommandWrap;
			var statement = ctx.Statement;
			var cmd = wrap.Command;
			cmd.CommandType = statement.CommandType;
			cmd.CommandTimeout = statement.Timeout;
			var parameterValues = ctx.Parameters;
			var commandText = this.GetCommandText(ctx, statement, parameterValues);
			foreach(var parameter in statement.Parameters) {
				var parameterValue = parameterValues[parameter.Name];
				if (parameter.Expandable) { //符合可扩展条件
					var array = parameterValue as Array;
					if (array != null) {//数组
						for (var i = 0; i < array.Length; i++) {
							commandText = this.BuildParameter(ctx, parameter, parameter.ParameterName + i, array.GetValue(i), commandText, cmd);
						}
					} else if(parameterValue != null) {//尝试转换成IDictionary形式
						var dict = parameterValue.ToDictionary();
						foreach (var key in dict.Keys) {
							var pn = Convert.ToString(key);
							if (string.IsNullOrEmpty(pn)) {
								continue;
							}
							this.BuildParameter(ctx, cmd, parameter, pn, dict[key]);
						}
					}//否则此选项无效
				} else {
					commandText = this.BuildParameter(ctx, parameter, parameter.ParameterName, parameterValue, commandText, cmd);
				}
			}
			cmd.CommandText = commandText;
			var dataService = ctx.DataService;
			if(dataService is DataServiceBase && ((DataServiceBase)dataService).Debug) {
				LogService.WriteLog(this, LogLevel.DEBUG, commandText + "\r\n" + JsonHelper.ToJson(parameterValues, false));
			}
			return cmd;
		}

		private string BuildParameter(IDbExecuteContext ctx, IParameterSetting parameter, string parameterName, object parameterValue, string commandText, DbCommand cmd) {
			var objectService = ObjectService.Current;
			var typeName = objectService.GetTypeName(parameter.ParameterTypeName);
			var parameterType = TypeHelper.CreateType(typeName, true);
			if (parameterValue != null) { //判断类型是否匹配
				var parameterRealType = parameterValue.GetType();
				if (parameterRealType.IsEnum && parameterType == typeof (int)) {} else if (parameterRealType.IsValueType) {} else if (!parameterType.IsInstanceOfType(parameterValue)) {
					throw new DataException(string.Format("参数 {0} 类型不匹配", parameter.Name));
				}
			} else { //获取缺省值
				parameterValue = parameter.GetDefaultValue(parameterType);
			}
			if (parameter.IsInline) {
				if (parameterValue != null) {
					commandText = commandText.Replace(parameterName, parameterValue.ToString());
				}
			} else {
				if (parameterValue != null && parameterValue is IObjectWrap) { //传出的参数？
					parameterValue = ((IObjectWrap) parameterValue).Value;
				}
				this.BuildParameter(ctx, cmd, parameter, parameterName, parameterValue);
			}
			return commandText;
		}

		protected virtual string GetCommandText(IDbExecuteContext ctx, IStatementSetting statement, IDictionary parameterValues) {
			return statement.StatementText.GetCommandText(statement, parameterValues);
		}

		protected virtual void BuildParameter(IDbExecuteContext ctx, DbCommand cmd, IParameterSetting parameter, string parameterName, object parameterValue) {
			var dbParameter = cmd.CreateParameter();
			dbParameter.ParameterName = parameterName;
			if(!string.IsNullOrEmpty(parameter.DbTypeName)) {
				ctx.DataStorage.SetDbType(dbParameter, parameter.DbTypeName);
			}
			var isNullable = parameter.IsNullable;
			if(isNullable && parameterValue == null) {
				parameterValue = DBNull.Value;
			}
			dbParameter.Direction = parameter.Direction;
			dbParameter.IsNullable = parameter.IsNullable;
			dbParameter.Size = parameter.Size;
			((IDbDataParameter)dbParameter).Scale = parameter.Scale;
			((IDbDataParameter)dbParameter).Precision = parameter.Precision;
			dbParameter.Value = parameterValue;
			cmd.Parameters.Add(dbParameter);
		}
	}
}
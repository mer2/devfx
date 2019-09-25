/* Copyright(c) 2005-2013 R2@DevFx.NET, License(LGPL) */

using DevFx.Data.Settings;
using DevFx.Logging;
using DevFx.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;

namespace DevFx.Data
{
	[Object]
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
			var definedNames = new List<string>();
			var parameters = statement.Parameters;
			if (parameters != null && parameters.Length > 0) {
				foreach(var parameter in parameters) {
					definedNames.Add(parameter.Name);
					var parameterValue = parameterValues[parameter.Name];
					if (parameter.Expandable) { //符合可扩展条件
						if (parameterValue is Array array) {//数组
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
			}
			if (statement.AutoParameters) {//自动生成SQL参数
				foreach (string key in parameterValues.Keys) {
					if (definedNames.Contains(key)) {
						continue;
					}
					this.BuildParameter(ctx, cmd, key, parameterValues[key]);
				}
			}
			cmd.CommandText = commandText;
			var dataService = ctx.DataService;
			if(dataService is DataServiceBase dsb && dsb.Debug) {
				LogService.Debug(commandText + Environment.NewLine + parameterValues.ToString(null, null), this);
			}
			return cmd;
		}

		protected virtual string GetCommandText(IDbExecuteContext ctx, IStatementSetting statement, IDictionary parameterValues) {
			if (statement.StatementText.Parser != null) {
				return statement.StatementText.Parser.GetCommandText(statement, parameterValues);
			}
			return statement.StatementText.CommandText;
		}

		private string BuildParameter(IDbExecuteContext ctx, IParameterSetting parameter, string parameterName, object parameterValue, string commandText, DbCommand cmd) {
			Type parameterType = null;
			if (!string.IsNullOrEmpty(parameter.ParameterTypeName)) {
				var objectService = ObjectService.Current;
				parameterType = objectService.GetOrCreateType(parameter.ParameterTypeName);
			}
			if (parameterValue != null) { //判断类型是否匹配
				var parameterRealType = parameterValue.GetType();
				if (parameterRealType.IsEnum && parameterType == typeof(int)) {
				} else if (parameterRealType.IsValueType) {
				} else if (parameterType != null && !parameterType.IsInstanceOfType(parameterValue)) {
					throw new DataException($"参数 {parameter.Name} 类型不匹配");
				}
			} else { //获取缺省值
				parameterValue = parameter.GetDefaultValue(parameterType);
			}
			if (parameter.IsInline) {
				if (parameterValue != null) {
					commandText = commandText.Replace(parameterName, parameterValue.ToString());
				}
			} else {
				if (parameterValue is IObjectWrap) { //传出的参数？
					parameterValue = ((IObjectWrap) parameterValue).Value;
				}
				this.BuildParameter(ctx, cmd, parameter, parameterName, parameterValue);
			}
			return commandText;
		}

		//通过配置和参数创建SQL参数
		protected virtual void BuildParameter(IDbExecuteContext ctx, DbCommand cmd, IParameterSetting parameter, string parameterName, object parameterValue) {
			var dbParameter = cmd.CreateParameter();
			dbParameter.ParameterName = parameterName;//这里的参数名已经有@前缀了
			var isNullable = parameter.IsNullable;
			if(isNullable && parameterValue == null) {
				parameterValue = DBNull.Value;
			}
			dbParameter.Value = parameterValue;
			if (!string.IsNullOrEmpty(parameter.DbTypeName)) {//显示指定了参数类型
				ctx.DataStorage.SetDbType(dbParameter, parameter.DbTypeName);
			} else {//未指定则从类型值里推断参数类型
				ctx.DataStorage.SetDbTypeFromValue(dbParameter, parameterValue);
			}

			dbParameter.Direction = parameter.Direction;
			dbParameter.IsNullable = parameter.IsNullable;
			dbParameter.Size = parameter.Size;
			((IDbDataParameter)dbParameter).Scale = parameter.Scale;
			((IDbDataParameter)dbParameter).Precision = parameter.Precision;
			cmd.Parameters.Add(dbParameter);
		}

		//仅通过参数创建SQL参数
		protected virtual void BuildParameter(IDbExecuteContext ctx, DbCommand cmd, string parameterName, object parameterValue) {
			if (parameterValue is DbParameter dbp) {//如果已经是参数类型，则直接添加
				cmd.Parameters.Add(dbp);
				return;
			}
			var dbParameter = cmd.CreateParameter();
			dbParameter.ParameterName = "@" + parameterName;//这里的参数名需要添加@前缀
			if (parameterValue == null) {
				parameterValue = DBNull.Value;
			}
			dbParameter.Value = parameterValue;
			ctx.DataStorage.SetDbTypeFromValue(dbParameter, parameterValue);
			cmd.Parameters.Add(dbParameter);
		}
	}
}
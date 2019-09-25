/* Copyright(c) 2005-2011 R2@DevFx.NET, License(LGPL) */

using System;
using System.Linq;
using System.Data;
using System.Data.Common;
using DevFx.Core;

namespace DevFx.Data.ResultHandlers
{
	public abstract class ResultHandlerBase : IResultHandler
	{
		protected ResultHandlerBase() { }
		protected ResultHandlerBase(IObjectMapper objectMapper) {
			this.ObjectMapper = objectMapper;
		}

		[Autowired(Required = true)]
		protected virtual IObjectService ObjectService { get; set; }
		protected virtual IObjectMapper ObjectMapper { get; set; }
		protected abstract object ExecuteResultInternal(IDbExecuteContext ctx, DbCommand command, Type resultType, object resultInstance);

		protected virtual Type GetResultType(IDbExecuteContext ctx, Type resultType, object resultInstance, Type defaultType) {
			return ResultHandlerFactory.GetResultType(ctx, resultType, resultInstance, defaultType);
		}

		protected virtual object ExecuteResult(IDbExecuteContext ctx, Type resultType, object resultInstance) {
			var cmd = this.GetDbCommand(ctx);
			var result = this.ExecuteResultInternal(ctx, cmd, resultType, resultInstance);
			return result;
		}

		protected virtual object HandleOutputOrReturnValue(IDbExecuteContext ctx, Type resultType, object resultInstance, DbCommand cmd, object executedResult) {
			//此方法由实现者自行调用，大部分情况下只有<see cref="NoneResultHandler" />会调用
			foreach(DbParameter dp in cmd.Parameters) {
				if(dp.Direction == ParameterDirection.InputOutput || dp.Direction == ParameterDirection.Output || dp.Direction == ParameterDirection.ReturnValue) {
					var ps = ctx.Statement.Parameters.SingleOrDefault(x => string.Compare(x.ParameterName, dp.ParameterName, StringComparison.InvariantCultureIgnoreCase) == 0);
					if(ps == null) {
						continue;
					}
					if(dp.Direction == ParameterDirection.ReturnValue) { //返回参数
						executedResult = dp.Value;
					} else {
						var valueWrap = ctx.Parameters[ps.Name] as IObjectWrap;
						if(valueWrap == null) {
							continue;
						}
						valueWrap.ChangeValue(dp.Value);
					}
				}
			}
			return executedResult;
		}

		#region CommandBuilder

		protected virtual DbCommand GetDbCommand(IDbExecuteContext ctx) {
			return ctx.DataStorage.GetCommand(ctx);
		}

		#endregion

		#region IResultHandler Members

		object IResultHandler.ExecuteResult(IDbExecuteContext ctx, Type resultType, object resultInstance) {
			return this.ExecuteResult(ctx, resultType, resultInstance);
		}

		#endregion
	}
}

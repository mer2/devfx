/* Copyright(c) 2005-2011 R2@DevFx.NET, License(LGPL) */

using System;
using System.Data;
using System.Data.Common;

namespace DevFx.Data.ResultHandlers
{
	[Object(Name = "Data.NoneResult")]
	public class NoneResultHandler : ResultHandlerBase
	{
		protected override object ExecuteResultInternal(IDbExecuteContext ctx, DbCommand command, Type resultType, object resultInstance) {
			var affectedRows = command.ExecuteNonQuery();
			//判断是否是存储过程，是则需要处理输出参数和返回参数
			if(command.CommandType == CommandType.StoredProcedure) {
				var result = this.HandleOutputOrReturnValue(ctx, resultType, resultInstance, command, affectedRows);
				return result;
			}
			return affectedRows;
		}
	}
}

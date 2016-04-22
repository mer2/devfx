/* Copyright(c) 2005-2011 R2@DevFx.NET, License(LGPL) */

using System;
using System.Data;
using System.Data.Common;

namespace HTB.DevFx.Data
{
	public class DataReaderResultHandler : ResultHandlerBase
	{
		#region Overrides of ResultHandlerBase

		protected override object ExecuteResultInternal(IDbExecuteContext ctx, DbCommand command, Type resultType, object resultInstance) {
			var defaultType = typeof(IDataReader);
			var type = this.GetResultType(ctx, resultType, resultInstance, defaultType);
			if (!defaultType.IsAssignableFrom(type)) {
				throw new DataException("要求返回的类型不正确");
			}
			return command.ExecuteReader(CommandBehavior.CloseConnection);
		}

		#endregion
	}
}

/* Copyright(c) 2005-2011 R2@DevFx.NET, License(LGPL) */

using System;
using System.Data.Common;

namespace HTB.DevFx.Data
{
	public class ScalarResultHandler : ResultHandlerBase
	{
		#region Overrides of ResultHandlerBase

		protected override object ExecuteResultInternal(IDbExecuteContext ctx, DbCommand command, Type resultType, object resultInstance) {
			var type = this.GetResultType(ctx, resultType, resultInstance, null);
			if(type != null && !(type.IsValueType || type == typeof(string))) {
				throw new DataException("要求返回的类型不正确");
			}
			var result = command.ExecuteScalar();
			if(!Convert.IsDBNull(result) && result != null && type != null) {
				var ut = Nullable.GetUnderlyingType(type) ?? type;
				result = Convert.ChangeType(result, ut);
			}
			return Convert.IsDBNull(result) ? null : result;
		}

		#endregion
	}
}

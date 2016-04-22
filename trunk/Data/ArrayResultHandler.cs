/* Copyright(c) 2005-2011 R2@DevFx.NET, License(LGPL) */

using System;
using System.Collections;
using System.Data;
using System.Data.Common;

namespace HTB.DevFx.Data
{
	public class ArrayResultHandler : ResultHandlerBase
	{
		protected ArrayResultHandler(IObjectMapper objectMapper) : base(objectMapper) { }

		#region Overrides of ResultHandlerBase

		protected override object ExecuteResultInternal(IDbExecuteContext ctx, DbCommand command, Type resultType, object resultInstance) {
			var type = this.GetResultType(ctx, resultType, resultInstance, null);
			if(type != null && !typeof(Array).IsAssignableFrom(type)) {
				throw new DataException("要求返回的类型不正确");
			}
			var elementType = type != null ? type.GetElementType() : null;
			var list = new ArrayList();
			using (var reader = command.ExecuteReader(CommandBehavior.SingleResult)) {
				while(reader.Read()) {
					var result = this.ObjectMapper.Mapping(ctx.Statement, reader.ToDictionary(), elementType, null);
					list.Add(result);
				}
			}
			return elementType == null ? list.ToArray() : list.ToArray(elementType);
		}

		#endregion
	}
}

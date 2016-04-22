/* Copyright(c) 2005-2011 R2@DevFx.NET, License(LGPL) */

using System;
using System.Data;
using System.Data.Common;

namespace HTB.DevFx.Data
{
	public class SingleResultHandler : ResultHandlerBase
	{
		public SingleResultHandler(IObjectMapper objectMapper) : base(objectMapper) { }

		#region Overrides of ResultHandlerBase

		protected override object ExecuteResultInternal(IDbExecuteContext ctx, DbCommand command, Type resultType, object resultInstance) {
			using (var reader = command.ExecuteReader(CommandBehavior.SingleRow)) {
				if(reader.Read()) {
					resultInstance = this.ObjectMapper.Mapping(ctx.Statement, reader.ToDictionary(), resultType, resultInstance);
				}
				return resultInstance;
			}
		}

		#endregion
	}
}

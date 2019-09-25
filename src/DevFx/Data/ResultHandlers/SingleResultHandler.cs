/* Copyright(c) 2005-2011 R2@DevFx.NET, License(LGPL) */

using DevFx.Utils;
using System;
using System.Data;
using System.Data.Common;

namespace DevFx.Data.ResultHandlers
{
	[Object(Name = "Data.SingleResult")]
	public class SingleResultHandler : ResultHandlerBase
	{
		[Autowired]
		protected SingleResultHandler(IObjectMapper objectMapper) : base(objectMapper) { }

		protected override object ExecuteResultInternal(IDbExecuteContext ctx, DbCommand command, Type resultType, object resultInstance) {
			using (var reader = command.ExecuteReader(CommandBehavior.SingleRow)) {
				if(reader.Read()) {
					resultInstance = this.ObjectMapper.Mapping(ctx.Statement, reader.ToDictionary(), resultType, resultInstance);
				}
				return resultInstance;
			}
		}
	}
}

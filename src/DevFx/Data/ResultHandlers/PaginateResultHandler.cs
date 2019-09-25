/* Copyright(c) 2005-2011 R2@DevFx.NET, License(LGPL) */

using DevFx.Data.Results;
using DevFx.Utils;
using System;
using System.Collections;
using System.Data;
using System.Data.Common;

namespace DevFx.Data.ResultHandlers
{
	[Object(Name = "Data.PaginateResult"), ResultHandler(typeof(IPaginateResult))]
	public class PaginateResultHandler : ResultHandlerBase
	{
		[Autowired]
		protected PaginateResultHandler(IObjectMapper objectMapper) : base(objectMapper) { }

		protected override object ExecuteResultInternal(IDbExecuteContext ctx, DbCommand command, Type resultType, object resultInstance) {
			var defaultType = typeof(IPaginateResult);
			var type = this.GetResultType(ctx, resultType, resultInstance, defaultType);
			if (!defaultType.IsAssignableFrom(type)) {
				throw new DataException("要求返回的类型不正确");
			}
			Type elementType = null;
			var types = type.GetGenericArguments();
			if(types != null && types.Length == 1) {
				elementType = types[0];
			}
			return this.ExecuteResultInternal(ctx, command, elementType);
		}

		protected virtual IPaginateResult ExecuteResultInternal(IDbExecuteContext ctx, DbCommand command, Type elementType) {
			PaginateResult result;
			if(elementType == null) {
				result = new PaginateResult();
			} else {
				var type = typeof(PaginateResult<>);
				var instanceType = type.MakeGenericType(elementType);
				result = (PaginateResult)TypeHelper.CreateObject(instanceType, null, true);
			}
			using (var reader = command.ExecuteReader(CommandBehavior.Default)) {
				if (reader.Read()) {
					result.Count = Convert.ToInt32(reader[0]);
				}
				var list = new ArrayList();
				if(reader.NextResult()) {
					while (reader.Read()) {
						list.Add(this.ObjectMapper.Mapping(ctx.Statement, reader.ToDictionary(), elementType, null));
					}
				}
				result.Items = elementType != null ? (object[])list.ToArray(elementType) : list.ToArray();
			}
			return result;
		}
	}
}

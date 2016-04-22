/* Copyright(c) 2005-2011 R2@DevFx.NET, License(LGPL) */

using System;
using System.Collections.Generic;
using System.Data.Common;
using HTB.DevFx.Core;
using HTB.DevFx.Data.Config;
using HTB.DevFx.Utils;

namespace HTB.DevFx.Data
{
	public class ResultHandlerSelector : ResultHandlerBase, IInitializable<IResultHandlerContextSetting>
	{
		protected ResultHandlerSelector() {}
		
		protected virtual IResultHandler DefaultResultHandler { get; private set; }
		protected virtual Dictionary<Type, IResultHandler> Handlers { get; private set; }

		protected virtual IResultHandler GetResultHandler(Type type) {
			foreach(var item in this.Handlers) {
				if(item.Key.IsAssignableFrom(type)) {
					return item.Value;
				}
			}
			return this.DefaultResultHandler;
		}

		public virtual void Init(IResultHandlerContextSetting setting) {
			this.DefaultResultHandler = this.ObjectService.GetObject<IResultHandler>(setting.DefaultHandlerName);
			this.Handlers = new Dictionary<Type, IResultHandler>();
			foreach(var handler in setting.ResultHandlers) {
				var typeName = this.ObjectService.GetTypeName(handler.ResultTypeName);
				var type = TypeHelper.CreateType(typeName, true);
				var resultHandler = this.ObjectService.GetObject<IResultHandler>(handler.ResultHandlerName);
				this.Handlers.Add(type, resultHandler);
			}
		}

		#region Overrides of ResultHandlerBase

		protected override object ExecuteResultInternal(IDbExecuteContext ctx, DbCommand command, Type resultType, object resultInstance) {
			throw new DataException("此操作无需实现。");
		}

		protected override object ExecuteResult(IDbExecuteContext ctx, Type resultType, object resultInstance) {
			var type = this.GetResultType(ctx, resultType, resultInstance, null);
			var handler = this.GetResultHandler(type);
			return handler.ExecuteResult(ctx, resultType, resultInstance);
		}

		#endregion
	}
}

/* Copyright(c) 2005-2011 R2@DevFx.NET, License(LGPL) */

using System;
using System.Linq;
using System.Collections.Generic;
using HTB.DevFx.Core;
using HTB.DevFx.Data.Config;
using HTB.DevFx.Utils;

namespace HTB.DevFx.Data
{
	public class ResultHandlerFactory : IResultHandlerFactory, IInitializable<IResultHandlerFactoryContextSetting>
	{
		protected ResultHandlerFactory() { }

		protected virtual IObjectService ObjectService { get; private set; }
		protected virtual IResultHandler DefaultResultHandler { get; private set; }
		protected virtual Dictionary<Type, IResultHandler> Handlers { get; private set; }
		protected virtual Dictionary<string, IResultModule> Modules { get; private set; }

		protected virtual void Init(IResultHandlerFactoryContextSetting setting) {
			this.ObjectService = DevFx.ObjectService.Current;
			if(setting.ModuleEnabled && setting.ResultModules != null && setting.ResultModules.Length > 0) {
				this.Modules = setting.ResultModules.Where(x => x.Enabled).ToDictionary(k => k.Name, v => this.ObjectService.GetOrCreateObject<IResultModule>(v.TypeName));
			} else {
				this.Modules = new Dictionary<string, IResultModule>();
			}
			this.Init(setting.ResultHandlerContext);
		}

		protected virtual void Init(IResultHandlerContextSetting setting) {
			this.DefaultResultHandler = this.ObjectService.GetObject<IResultHandler>(setting.DefaultHandlerName);
			this.Handlers = new Dictionary<Type, IResultHandler>();
			foreach (var handler in setting.ResultHandlers) {
				var typeName = this.ObjectService.GetTypeName(handler.ResultTypeName);
				var type = TypeHelper.CreateType(typeName, true);
				var resultHandler = this.ObjectService.GetObject<IResultHandler>(handler.ResultHandlerName);
				this.Handlers.Add(type, resultHandler);
			}
		}

		protected virtual IResultHandler GetResultHandler(Type type) {
			foreach (var item in this.Handlers) {
				if (item.Key.IsAssignableFrom(type)) {
					return item.Value;
				}
			}
			return this.DefaultResultHandler;
		}

		protected virtual object FactoryExecuteResult(IDbExecuteContext ctx, Type resultType, object resultInstance) {
			IResultHandler handler;
			var handlerName = ctx.Statement.ResultHandlerName;
			if(string.IsNullOrEmpty(handlerName)) {
				var type = GetResultType(ctx, resultType, resultInstance, null);
				handler = this.GetResultHandler(type);
			} else {
				handler = this.ObjectService.GetOrCreateObject<IResultHandler>(handlerName);
			}
			var resultContext = new DbResultContext(ctx, resultType, resultInstance, handler, null);
			foreach(var module in this.Modules.Values) {
				module.OnResultExecute(resultContext);
				if(resultContext.ResultHandled) {
					break;
				}
			}
			handler = resultContext.ResultHandler;
			if(handler == null) {
				throw new DataException("ResultHandler Not Found");
			}
			if (resultContext.ResultHandled) {
				return resultContext.ResultInstance;
			}
			if (resultType == null && resultInstance == null) {//都没有指定？则使用无返回值的处理器
				var noneHandler = this.ObjectService.GetObject<IResultHandler>("@Data.NoneResultHandler");
				if (noneHandler != null) {
					handler = noneHandler;
				}
			}
			return handler.ExecuteResult(ctx, resultType, resultInstance);
		}

		public static Type GetResultType(IDbExecuteContext ctx, Type resultType, object resultInstance, Type defaultType) {
			if (resultType == null) {
				if (resultInstance != null) {
					resultType = resultInstance.GetType();
				}
				if (resultType == null) {
					resultType = defaultType;
				}
			}
			return resultType;
		}

		#region IResultHandlerFactory Members

		object IResultHandlerFactory.ExecuteResult(IDbExecuteContext ctx, Type resultType, object resultInstance) {
			return this.FactoryExecuteResult(ctx, resultType, resultInstance);
		}

		#endregion

		#region IInitializable<IResultHandlerFactoryContextSetting> Members

		void IInitializable<IResultHandlerFactoryContextSetting>.Init(IResultHandlerFactoryContextSetting setting) {
			this.Init(setting);
		}

		#endregion
	}
}

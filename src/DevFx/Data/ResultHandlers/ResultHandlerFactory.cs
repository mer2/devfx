/* Copyright(c) 2005-2011 R2@DevFx.NET, License(LGPL) */

using System;
using System.Collections.Generic;
using DevFx.Core;
using DevFx.Data.Settings;

namespace DevFx.Data.ResultHandlers
{
	[Object(Name = "Data.DefaultResultHandlerFactory")]
	public class ResultHandlerFactory : IResultHandlerFactory
	{
		protected ResultHandlerFactory() { }
		protected virtual IObjectService ObjectService { get; private set; }
		protected virtual IResultHandler DefaultResultHandler { get; private set; }
		protected virtual Dictionary<Type, IResultHandler> Handlers { get; private set; }
		protected virtual IResultModule[] Modules { get; private set; }

		public virtual void Init(IObjectService objectService, IResultHandlerFactoryContextSetting setting) {
			this.ObjectService = objectService;

			if (setting.ModuleEnabled) {
				this.Modules = this.ObjectService.GetObjects<IResultModule>();
			}

			this.DefaultResultHandler = this.ObjectService.GetObject<IResultHandler>(setting.DefaultHandlerName);
			var handlers = this.Handlers = new Dictionary<Type, IResultHandler>();
			var coreAttributes = ((IObjectServiceInternal)objectService).CoreAttributes;
			coreAttributes.TryGetValue(typeof(ResultHandlerAttribute), out var attributes);
			if(attributes != null && attributes.Count > 0) {
				foreach(ResultHandlerAttribute attribute in attributes) {
					if (this.ObjectService.GetObject(attribute.OwnerType) is IResultHandler resultHandler) {
						handlers.Add(attribute.HandleType, resultHandler);
					}
				}
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
			var returnValue = FactoryExecuteResultInternal(ctx, resultType, resultInstance);
			if (returnValue == null && resultType.IsValueType) {
				if (!resultType.IsGenericType || resultType.GetGenericTypeDefinition() != typeof(Nullable<>)) {
					throw new DataException($"期待返回的类型是值类型[{resultType.Name}]，但返回的是null，无法转换。");
				}
			}
			return returnValue;
		}

		protected virtual object FactoryExecuteResultInternal(IDbExecuteContext ctx, Type resultType, object resultInstance) {
			IResultHandler handler;
			var handlerName = ctx.Statement.ResultHandlerName;
			if(string.IsNullOrEmpty(handlerName)) {
				var type = GetResultType(ctx, resultType, resultInstance, null);
				handler = this.GetResultHandler(type);
			} else {
				handler = this.ObjectService.GetOrCreateObject<IResultHandler>(handlerName);
			}
			var resultContext = new DbResultContext(ctx, resultType, resultInstance, handler, null);
			if(this.Modules != null && this.Modules.Length > 0) {
				foreach(var module in this.Modules) {
					module.OnResultExecute(resultContext);
					if(resultContext.ResultHandled) {
						break;
					}
				}
			}
			if (resultContext.ResultHandled) {
				return resultContext.ResultInstance;
			}
			handler = resultContext.ResultHandler;
			if(handler == null) {
				throw new DataException("ResultHandler Not Found");
			}
			if (resultType == null && resultInstance == null) {//都没有指定？则使用无返回值的处理器
				var noneHandler = this.ObjectService.GetObject<NoneResultHandler>();
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
	}
}

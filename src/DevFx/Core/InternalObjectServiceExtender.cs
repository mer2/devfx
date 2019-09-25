using System;
using System.Collections;
using DevFx.Core;
using DevFx.Exceptions;
using DevFx.Logging;

namespace DevFx.Configuration
{
	[ObjectServiceExtender]
	internal class InternalObjectServiceExtender : IObjectExtender<IObjectService>
	{
		public void Init(IObjectService objectService, IDictionary items) {
			objectService.SetupCompleted += this.ObjectServiceOnSetupCompleted;
			objectService.ObjectCreated += this.ObjectServiceOnObjectCreated;
			objectService.Error += this.ObjectServiceOnError;
			objectService.Disposing += this.ObjectServiceOnDisposing;
		}

		//处理StarterAttribute
		private void ObjectServiceOnSetupCompleted(IObjectServiceContext ctx) {
			StarterAttribute.Init(ctx.Items);
		}

		//未处理的异常
		private void ObjectServiceOnError(IObjectServiceContext ctx, Exception e) {
			ExceptionService.Publish(e);
		}

		//把缓存中的日志保存
		private void ObjectServiceOnDisposing(IObjectServiceContext ctx) {
			var logService = ctx.ObjectService.GetObject<LogService>();
			logService?.OnTimerInternal();
		}

		private void ObjectServiceOnObjectCreated(IObjectBuilderContext ctx) {
			//处理实现IInitializable<T>的类型
			var instance = ctx.ObjectInstance;
			if(instance == null) {
				return;
			}
			var type = instance.GetType();
			var interfaces = type.GetInterfaces();
			if(interfaces != null) {
				var objectService = ctx.ObjectService;
				foreach (var intf in interfaces) {
					if (!intf.IsGenericType) {
						continue;
					}
					if (intf.GetGenericTypeDefinition() != typeof(IInitializable<>)) {
						continue;
					}
					var args = intf.GetGenericArguments();
					var settingType = args[0];
					var settingInstance = objectService.GetObject(settingType);
					var method = intf.GetMethod("Init");
					if (method != null) {
						method.Invoke(instance, new object[] { settingInstance });
					}
				}
			}

			(instance as IInitializable)?.Init();
		}
	}
}
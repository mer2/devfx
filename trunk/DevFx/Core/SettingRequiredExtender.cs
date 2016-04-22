/* Copyright(c) 2005-2011 R2@DevFx.NET, License(LGPL) */

using System.Runtime.Remoting;
using HTB.DevFx.Config;
using HTB.DevFx.Core.Config;
using HTB.DevFx.Utils;

namespace HTB.DevFx.Core
{
	partial class ObjectServiceBase
	{
		/// <summary>
		/// <see cref="ObjectService"/>的扩展，实现配置自动注入
		/// </summary>
		internal class SettingRequiredExtender : ObjectServiceExtenderBase
		{
			protected SettingRequiredExtender() { }

			protected override void Init(IObjectService objectService) {
				base.Init(objectService);
				objectService.ObjectBuilder.ObjectCreated += this.ObjectServiceOnObjectCreated;
			}

			private void ObjectServiceOnObjectCreated(IObjectBuilderContext builderContext) {
				var instance = builderContext.ObjectInstance;
				if(instance == null) {
					return;
				}
				if(RemotingServices.IsTransparentProxy(instance)) {
					return;
				}
				var objectService = this.ObjectService as ObjectServiceBase;
				if(objectService == null) {
					return;
				}

				SetObjectSetting(objectService, builderContext.ObjectSetting, instance);

				if(instance is IInitializable) {
					((IInitializable)instance).Init();
				}
			}

			private static IConfigSettingElement GetSetting(ObjectServiceBase objectService, IObjectSetting objectSetting, IConfigSettingElement setting) {
				setting.ConfigSetting = objectService.GetObjectSettingInternal(objectSetting);
				var handlerName = objectSetting.SettingSet.Value;
				if(!string.IsNullOrEmpty(handlerName)) {
					var handler = objectService.GetOrCreateObject<ISettingHandler>(handlerName);
					if(handler != null) {
						setting = handler.GetSetting(setting);
					}
				}
				return setting;
			}

			private static void SetObjectSetting(ObjectServiceBase objectService, IObjectSetting objectSetting, object instance) {
				if(objectSetting.SettingSet == null || string.IsNullOrEmpty(objectSetting.SettingSet.TypeName)) {
					return;
				}
				var typeName = objectService.GetTypeNameInternal(objectSetting.SettingSet.TypeName, objectSetting.Namespace);
				var setting = (IConfigSettingElement)TypeHelper.CreateObject(typeName, typeof(IConfigSettingElement), false);
				setting = GetSetting(objectService, objectSetting, setting);
				if (setting == null) {
					return;
				}
				if (instance is ISettingRequired) {
					((ISettingRequired)instance).Setting = setting;
				}
				var settingType = setting.GetType();
				var iinitializableType = typeof (IInitializable<>).MakeGenericType(settingType);
				if (iinitializableType.IsInstanceOfType(instance)) {
					var mi = iinitializableType.GetMethod("Init");
					object returnValue;
					TypeHelper.TryInvoke(instance, mi, out returnValue, false, setting);
				}
				var isettingRequiredType = typeof(ISettingRequired<>).MakeGenericType(settingType);
				if(isettingRequiredType.IsInstanceOfType(instance)) {
					var property = isettingRequiredType.GetProperty("Setting");
					try {
						property.SetValue(instance, setting, null);
					} catch { }
				}
			}
		}
	}
}

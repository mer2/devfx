/* Copyright(c) 2005-2011 R2@DevFx.NET, License(LGPL) */

using System;
using System.Reflection;
using System.Runtime.Remoting;
using HTB.DevFx.Config;
using HTB.DevFx.Core;
using HTB.DevFx.Esb.Config;
using HTB.DevFx.Utils;

namespace HTB.DevFx.Esb
{
	/// <summary>
	/// 配置注入扩展
	/// </summary>
	public class SettingRequiredExtender : IObjectExtender<IServiceLocator>
	{
		/// <summary>
		/// ServiceLocator初始化时调用
		/// </summary>
		void IObjectExtender<IServiceLocator>.Init(IServiceLocator serviceLocator) {
			serviceLocator.ObjectBuilt += OnObjectBuilt;
		}

		private static void OnObjectBuilt(IObjectBuildContext ctx) {
			var instance = ctx.ObjectInstance;
			if (instance == null) {
				return;
			}
			SetObjectSetting(ctx.ServiceLocator, ctx.ObjectSetting, instance);
			if (!RemotingServices.IsTransparentProxy(instance) && instance is IInitializable) {
				((IInitializable)instance).Init();
			}
		}
	
		private static void SetObjectSetting(IServiceLocator serviceLocator, IObjectSetting objectSetting, object instance) {
			var settingSet = objectSetting.SettingSet;
			IConfigSettingElement setting;
			if (settingSet == null || string.IsNullOrEmpty(settingSet.TypeName)) {
				return;
			}
			var typeName = serviceLocator.GetTypeName(settingSet.TypeName);
			if (instance is ISettingRequired) {
				setting = (IConfigSettingElement)TypeHelper.CreateObject(typeName, typeof(IConfigSettingElement), false);
				if (setting != null) {
					setting.ConfigSetting = GetCustomSetting(objectSetting);
					((ISettingRequired)instance).Setting = setting;
				}
				return;
			}
			var currentType = instance.GetType();
			Type initorType = null, requiredType = null;
			try {
				initorType = currentType.GetInterface(typeof(IInitializable<>).FullName);
				requiredType = currentType.GetInterface(typeof(ISettingRequired<>).FullName);
			} catch (AmbiguousMatchException) { }
			var type = initorType ?? requiredType;
			if (type == null) {
				return;
			}
			var settingType = type.GetGenericArguments()[0];
			var settingInstanceType = TypeHelper.CreateType(typeName, settingType, false);
			if (settingInstanceType == null) {
				return;
			}
			setting = (IConfigSettingElement)TypeHelper.CreateObject(settingInstanceType, typeof(IConfigSettingElement), false);
			if (setting == null) {
				return;
			}
			setting.ConfigSetting = GetCustomSetting(objectSetting);
			if (type == initorType) {
				TypeHelper.Invoke(instance, "Init", setting);
			} else {
				TypeHelper.SetPropertyValue(instance, "Setting", setting);
			}
		}

		/// <summary>
		/// 获取定制后的配置节
		/// </summary>
		/// <param name="objectSetting">对象配置信息</param>
		/// <returns>定制后的配置节</returns>
		public static IConfigSetting GetCustomSetting(IObjectSetting objectSetting) {
			if (objectSetting != null) {
				var settingSet = objectSetting.SettingSet;
				var settingName = settingSet != null ? settingSet.Name : "setting";
				return objectSetting.ConfigSetting.GetChildSetting(settingName);
			}
			return null;
		}

		/// <summary>
		/// 获取定制后的强类型配置节
		/// </summary>
		/// <typeparam name="T">强类型配置类型</typeparam>
		/// <param name="objectSetting">对象配置信息</param>
		/// <returns>强类型配置节</returns>
		public static T GetCustomSetting<T>(IObjectSetting objectSetting) {
			var setting = GetCustomSetting(objectSetting);
			if(setting != null) {
				var settingSet = objectSetting.SettingSet;
				var typeName = ServiceLocator.GetTypeName(settingSet.TypeName);
				var type = TypeHelper.CreateType(typeName, typeof(T), false);
				if(type != null) {
					var settingInstance = (IConfigSettingElement)TypeHelper.CreateObject(type, typeof(IConfigSettingElement), false);
					if(settingInstance != null) {
						settingInstance.ConfigSetting = setting;
						return (T)settingInstance;
					}
				}
			}
			return default(T);
		}

		/// <summary>
		/// 转换成定制后的强类型配置节
		/// </summary>
		/// <typeparam name="T">强类型配置类型</typeparam>
		/// <param name="objectSetting">对象配置信息</param>
		/// <returns>强类型配置节</returns>
		public static T ToCustomSetting<T>(IObjectSetting objectSetting) {
			var setting = GetCustomSetting(objectSetting);
			if (setting != null) {
				var settingInstance = (IConfigSettingElement)TypeHelper.CreateObject(typeof(T), typeof(IConfigSettingElement), false);
				if (settingInstance != null) {
					settingInstance.ConfigSetting = setting;
					return (T)settingInstance;
				}
			}
			return default(T);
		}
	}
}

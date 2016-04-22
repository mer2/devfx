/* Copyright(c) 2005-2011 R2@DevFx.NET, License(LGPL) */

using System;
using HTB.DevFx.Utils;

namespace HTB.DevFx.Config
{
	public static class ConfigSettingExtensions
	{
		public static T GetObject<T>(this IConfigSettingElement settingElement, string objectSettingName) {
			return settingElement.GetObject<T>(objectSettingName, null);
		}

		public static T GetObject<T>(this IConfigSettingElement settingElement, string objectSettingName, bool autoInit) {
			return settingElement.GetObject<T>(objectSettingName, autoInit, null);
		}

		public static T GetObject<T>(this IConfigSettingElement settingElement, string objectSettingName, Action<T> initializer) {
			return settingElement.GetObject(objectSettingName, true, initializer);
		}

		public static T GetObject<T>(this IConfigSettingElement settingElement, string objectSettingName, bool autoInit, Action<T> initializer) {
			var objectAlias = settingElement.GetSetting(objectSettingName);
			var instance = ObjectService.Current.GetOrCreateObject<T>(objectAlias);
			if (!Equals(instance, default(T)) && initializer != null) {
				initializer(instance);
			}
			if (autoInit && instance is ISettingInitialize) {
				((ISettingInitialize)instance).Init(settingElement.ConfigSetting);
			}
			return instance;
		}

		public static object GetObject(this IConfigSettingElement settingElement, string objectSettingName, bool autoInit) {
			var objectAlias = settingElement.GetSetting(objectSettingName);
			var instance = ObjectService.Current.GetOrCreateObject(objectAlias);
			if (autoInit && instance is ISettingInitialize) {
				((ISettingInitialize)instance).Init(settingElement.ConfigSetting);
			}
			return instance;
		}

		public static T GetCachedObject<T>(this IConfigSettingElement settingElement, string objectSettingName) {
			return settingElement.GetCachedObject<T>(objectSettingName, null);
		}

		public static T GetCachedObject<T>(this IConfigSettingElement settingElement, string objectSettingName, bool autoInit) {
			return settingElement.GetCachedObject<T>(objectSettingName, autoInit, null);
		}

		public static T GetCachedObject<T>(this IConfigSettingElement settingElement, string objectSettingName, Action<T> initializer) {
			return settingElement.GetCachedObject(objectSettingName, true, initializer);
		}

		public static T GetCachedObject<T>(this IConfigSettingElement settingElement, string objectSettingName, bool autoInit, Action<T> initializer) {
			return settingElement.GetObjectContext(objectSettingName, () => settingElement.GetObject(objectSettingName, autoInit, initializer));
		}

		public static object GetCachedObject(this IConfigSettingElement settingElement, string objectSettingName, bool autoInit) {
			return settingElement.GetObjectContext(objectSettingName, () => settingElement.GetObject(objectSettingName, autoInit));
		}
	}
}

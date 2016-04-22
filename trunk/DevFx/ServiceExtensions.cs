/* Copyright(c) 2005-2011 R2@DevFx.NET, License(LGPL) */

using System;
using System.Collections;
using HTB.DevFx.Config;
using HTB.DevFx.Core;
using HTB.DevFx.Core.Config;
using HTB.DevFx.Utils;

namespace HTB.DevFx
{
	public static class ServiceExtensions
	{
		#region GetObjectConstructorSetting

		public static IObjectSetting GetObjectConstructorSetting(this IObjectService objectService, Type objectType) {
			return objectService.GetObjectConstructorSetting(objectType, ObjectServiceBase.GlobalObjectNamespaceName);
		}

		public static IObjectSetting GetObjectConstructorSetting(this IObjectService objectService, string objectAlias) {
			return objectService.GetObjectConstructorSetting(objectAlias, ObjectServiceBase.GlobalObjectNamespaceName);
		}

		public static IObjectSetting GetObjectConstructorSetting(this IObjectService objectService, string objectAlias, string spaceName) {
			return objectService.GetObjectConstructorSetting((object)objectAlias, spaceName);
		}

		private static IObjectSetting GetObjectConstructorSetting(this IObjectService objectService, object objectKey, string spaceName) {
			if (objectService == null || !(objectService is ObjectServiceBase)) {
				return null;
			}
			var osb = (ObjectServiceBase)objectService;
			var container = osb.GetLifetimeContainer(objectKey, spaceName);
			return container == null ? null : container.ObjectSetting;
		}

		#endregion

		#region GetOrCreateObject

		public static object GetOrCreateObject(this IObjectService objectService, string objectAlias) {
			if (string.IsNullOrEmpty(objectAlias)) {
				return null;
			}
			var instance = objectService.GetObject(objectAlias);
			if (instance == null) {
				var typeName = objectService.GetTypeName(objectAlias);
				if (!string.IsNullOrEmpty(typeName)) {
					instance = TypeHelper.CreateObject(typeName, null, false);
				}
			}
			return instance;
		}

		public static object GetOrCreateObject(this IObjectService objectService, string objectAlias, IDictionary parameters) {
			if (string.IsNullOrEmpty(objectAlias)) {
				return null;
			}
			var instance = objectService.GetObject(objectAlias, parameters);
			if (instance == null) {
				var typeName = objectService.GetTypeName(objectAlias);
				if (!string.IsNullOrEmpty(typeName)) {
					instance = TypeHelper.CreateObject(typeName, null, false);
				}
			}
			return instance;
		}

		public static T GetOrCreateObject<T>(this IObjectService objectService, string objectAlias) {
			var aliasNull = string.IsNullOrEmpty(objectAlias);
			var instance = aliasNull ? objectService.GetObject<T>() : objectService.GetObject<T>(objectAlias);
			if (Equals(instance, default(T))) {
				if (!aliasNull) {
					var typeName = objectService.GetTypeName(objectAlias);
					if (!string.IsNullOrEmpty(typeName)) {
						instance = (T)TypeHelper.CreateObject(typeName, typeof(T), false);
					}
				}
				if (Equals(instance, default(T))) {
					instance = (T)TypeHelper.CreateObject(typeof(T), null, false);
				}
			}
			return instance;
		}

		public static T GetOrCreateObject<T>(this IObjectService objectService, string objectAlias, IDictionary parameters) {
			var aliasNull = string.IsNullOrEmpty(objectAlias);
			var instance = aliasNull ? objectService.GetObject<T>(parameters) : objectService.GetObject<T>(objectAlias, parameters);
			if (Equals(instance, default(T))) {
				if (!aliasNull) {
					var typeName = objectService.GetTypeName(objectAlias);
					if (!string.IsNullOrEmpty(typeName)) {
						instance = (T)TypeHelper.CreateObject(typeName, typeof(T), false);
					}
				}
				if (Equals(instance, default(T))) {
					instance = (T)TypeHelper.CreateObject(typeof(T), null, false);
				}
			}
			return instance;
		}

		#endregion

		#region GetObjectTypedSetting
		
		public static TSetting GetObjectTypedSetting<TObject, TSetting>(this IObjectService objectService) where TSetting : class {
			return objectService.GetObjectTypedSetting<TSetting>(typeof(TObject));
		}

		public static TSetting GetObjectTypedSetting<TSetting>(this IObjectService objectService, Type type) where TSetting : class {
			return objectService.GetObjectTypedSetting<TSetting>((object)type, false);
		}

		public static TSetting GetObjectTypedSetting<TSetting>(this IObjectService objectService, string objectAlias) where TSetting : class {
			return objectService.GetObjectTypedSetting<TSetting>(objectAlias, false);
		}

		public static TSetting GetObjectTypedSetting<TSetting>(this IObjectService objectService, string objectAlias, string spaceName) where TSetting : class {
			return objectService.GetObjectTypedSetting<TSetting>(objectAlias, spaceName, false);
		}

		public static TSetting GetObjectTypedSetting<TObject, TSetting>(this IObjectService objectService, bool cached) where TSetting : class {
			return objectService.GetObjectTypedSetting<TSetting>(typeof(TObject), cached);
		}

		public static TSetting GetObjectTypedSetting<TSetting>(this IObjectService objectService, Type type, bool cached) where TSetting : class {
			return objectService.GetObjectTypedSetting<TSetting>((object)type, cached);
		}

		public static TSetting GetObjectTypedSetting<TSetting>(this IObjectService objectService, string objectAlias, bool cached) where TSetting : class {
			return objectService.GetObjectTypedSetting<TSetting>((object)objectAlias, cached);
		}

		public static TSetting GetObjectTypedSetting<TSetting>(this IObjectService objectService, string objectAlias, string spaceName, bool cached) where TSetting : class {
			return objectService.GetObjectTypedSetting<TSetting>((object)objectAlias, spaceName, cached);
		}

		private static TSetting GetObjectTypedSetting<TSetting>(this IObjectService objectService, object objectKey, bool cached) where TSetting : class {
			return objectService.GetObjectTypedSetting<TSetting>(objectKey, ObjectServiceBase.GlobalObjectNamespaceName, cached);
		}

		internal static readonly object ObjectTypedSettingCachedKey = new object();
		private static TSetting GetObjectTypedSetting<TSetting>(this IObjectService objectService, object objectKey, string spaceName, bool cached) where TSetting : class {
			var objectSetting = objectService.GetObjectConstructorSetting(objectKey, spaceName);
			return cached && objectSetting != null ? objectSetting.GetObjectContext(ObjectTypedSettingCachedKey, () => objectService.GetObjectTypedSetting<TSetting>(objectSetting)) : objectService.GetObjectTypedSetting<TSetting>(objectSetting);
		}

		private static TSetting GetObjectTypedSetting<TSetting>(this IObjectService objectService, IObjectSetting objectSetting) where TSetting : class {
			if(objectSetting == null) {
				return null;
			}
			var osb = (ObjectServiceBase)objectService;
			if (objectSetting.SettingSet != null && !string.IsNullOrEmpty(objectSetting.SettingSet.TypeName)) {
				var typeName = osb.GetTypeNameInternal(objectSetting.SettingSet.TypeName, objectSetting.Namespace);
				var settingType = typeof(TSetting);
				var settingInstanceType = TypeHelper.CreateType(typeName, settingType, false);
				if (settingInstanceType != null) {
					var setting = (IConfigSettingElement)TypeHelper.CreateObject(settingInstanceType, typeof(IConfigSettingElement), false);
					if (setting != null) {
						setting.ConfigSetting = osb.GetObjectSettingInternal(objectSetting);
						return (TSetting)setting;
					}
				}
			}
			return null;
		}

		#endregion
	}
}

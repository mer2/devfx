/* Copyright(c) 2005-2011 R2@DevFx.NET, License(LGPL) */

using System;
using HTB.DevFx.Config;
using HTB.DevFx.Core;
using HTB.DevFx.Core.Config;
using HTB.DevFx.Exceptions;
using HTB.DevFx.Utils;

namespace HTB.DevFx
{
	public abstract class ObjectService : ObjectServiceBase
	{
		#region IObjectService Static Members

		public static void Init() {
			Current.ToString();
		}

		public static string GetTypeName(string typeAlias) {
			return Current.GetTypeName(typeAlias);
		}

		public static object GetObject(string objectAlias) {
			return Current.GetObject(objectAlias);
		}

		public static T GetObject<T>(string objectAlias) {
			return Current.GetObject<T>(objectAlias);
		}

		public static T GetObject<T>() {
			return Current.GetObject<T>();
		}

		public static object GetObject(Type type) {
			return Current.GetObject(type);
		}

		public static T CreateObject<T>(params object[] args) {
			return Current.CreateObject<T>(args);
		}

		public static object CreateObject(Type type, params object[] args) {
			return Current.CreateObject(type, args);
		}

		public static object CreateObject(string objectAlias, params object[] args) {
			return Current.CreateObject(objectAlias, null, args);
		}

		public static IConfigSetting GetObjectSetting<T>() {
			return Current.GetObjectSetting(typeof(T));
		}

		public static IConfigSetting GetObjectSetting(Type type) {
			return Current.GetObjectSetting(type);
		}

		public static IConfigSetting GetObjectSetting(string objectAlias) {
			return Current.GetObjectSetting(objectAlias);
		}

		public static T[] GetObjects<T>() {
			return Current.GetObjects<T>();
		}

		public static object[] GetObjects(Type type) {
			return Current.GetObjects(type);
		}

		#endregion

		#region Setup ObjectService

		private volatile static IObjectService currentInstance;
		private static bool currentInitializing;
		public static IObjectService Current {
			get {
				if (currentInstance == null) {
					lock(typeof(ObjectService)) {
						if (currentInstance == null) {
							if(currentInitializing) {
								throw new ExceptionBase(-1, "ObjectService正在初始化，不可用。");
							}
							currentInitializing = true;
							try {
								currentInstance = Setup();
							} finally {
								currentInitializing = false;
							}
							((IObjectServiceInternal)currentInstance).InitCompleted();
						}
					}
				}
				return currentInstance;
			}
		}

		private static IObjectService Setup() {
			IStartupSetting setting = ConfigSectionHandler.GetConfig("htb.devfx").ToSetting<StartupSetting>("startup");
			string configServiceTypeName = null;
			string objectServiceTypeName = null;
			if (setting != null && setting.CoreSetting != null) {
				if (setting.CoreSetting.ConfigServiceSetting != null) {
					configServiceTypeName = setting.CoreSetting.ConfigServiceSetting.TypeName;
				}
				if (setting.CoreSetting.ObjectServiceSetting != null) {
					objectServiceTypeName = setting.CoreSetting.ObjectServiceSetting.TypeName;
				}
			}

			IConfigService configServiceInstance = null;
			if (!string.IsNullOrEmpty(configServiceTypeName)) {
				configServiceInstance = (IConfigService)TypeHelper.CreateObject(configServiceTypeName, typeof(IConfigService), false);
			}
			if (configServiceInstance == null) {
				configServiceInstance = Config.ConfigService.Default;
			}

			if (configServiceInstance is IConfigServiceInternal) {
				((IConfigServiceInternal)configServiceInstance).Init(setting);
			} else if (configServiceInstance is IInitializable) {
				((IInitializable)configServiceInstance).Init();
			}

			IObjectService objectServiceInstance = null;
			if (!string.IsNullOrEmpty(objectServiceTypeName)) {
				objectServiceInstance = (IObjectService)TypeHelper.CreateObject(objectServiceTypeName, typeof(IObjectService), false);
			}
			if (objectServiceInstance == null) {
				objectServiceInstance = new DefaultObjectService();
			}
			if(objectServiceInstance is IObjectServiceInternal) {
				((IObjectServiceInternal)objectServiceInstance).Init(configServiceInstance);
			} else if (objectServiceInstance is IInitializable) {
				((IInitializable)objectServiceInstance).Init();
			}
			return objectServiceInstance;
		}

		internal class DefaultObjectService : ObjectService
		{
		}

		#endregion
	}
}
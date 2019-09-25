using System;
using System.Collections;
using System.Collections.Specialized;
using System.Reflection;
using System.Threading.Tasks;
using DevFx.Configuration;
using DevFx.Core;
using DevFx.Core.Settings;
using DevFx.Utils;

namespace DevFx
{
	public abstract class ObjectService : ObjectServiceBase
	{
		public static void Init() {
			Current.ToString();
		}

		public static T GetObject<T>() where T : class {
			return Current.GetObject<T>();
		}

		#region Setup ObjectService

		private static volatile IObjectService currentInstance;
		private static bool currentInitializing;
		public static IObjectService Current {
			get {
				if(currentInstance == null) {
					lock(typeof(ObjectService)) {
						if(currentInstance == null) {
							if(currentInitializing) {
								throw new Exception("ObjectService正在初始化，不可用。");
							}
							currentInitializing = true;
							var items = new HybridDictionary();//在初始化过程中可供临时保存的字典
							try {
								currentInstance = Setup(items);
								var objectService = (ObjectService)currentInstance;
								objectService.InitCompletedInternal(items);
								Task.Run(() => objectService.SetupCompletedInternal(items));
							} finally {
								currentInitializing = false;
							}
						}
					}
				}
				return currentInstance;
			}
		}

		private static IObjectService Setup(IDictionary items) {
			var appSetting = ConfigHelper.CreateFromXmlFile("app.config") ?? ConfigHelper.CreateFromXmlFile(Assembly.GetEntryAssembly().ManifestModule.Name + ".config");
			var startupSetting = appSetting?.ToSetting<StartupSetting>($"{ConfigHelper.RootSettingName}/startup");
			var setting = ConfigServiceBase.Init(startupSetting, appSetting);

			var objectServiceInstance = new DefaultObjectService();
			objectServiceInstance.Init(setting, items);
			return objectServiceInstance;
		}

		internal class DefaultObjectService : ObjectService
		{
		}

		#endregion
	}

	public static class ObjectServiceExtensions
	{
		public static Type GetOrCreateType(this IObjectService objectService, string typeName) {
			var type = objectService.GetType(typeName);
			if(type == null) {
				type = TypeHelper.CreateType(typeName, false);
			}
			return type;
		}

		public static IObjectServiceInternal AsObjectServiceInternal(this IObjectService objectService) {
			return (IObjectServiceInternal)objectService;
		}

		#region GetOrCreateObject

		public static object GetOrCreateObject(this IObjectService objectService, string objectAlias) {
			if (string.IsNullOrEmpty(objectAlias)) {
				return null;
			}
			var instance = objectService.GetObject(objectAlias);
			if (instance == null) {
				var type = objectService.GetOrCreateType(objectAlias);
				if (type != null) {
					instance = objectService.CreateObject(type);
				}
			}
			return instance;
		}

		public static object GetOrCreateObject(this IObjectService objectService, Type objectType) {
			if (objectType == null) {
				return null;
			}
			var instance = objectService.GetObject(objectType) ?? objectService.CreateObject(objectType);
			return instance;
		}

		public static T GetOrCreateObject<T>(this IObjectService objectService, string objectAlias) where T : class {
			var aliasNull = string.IsNullOrEmpty(objectAlias);
			var instance = aliasNull ? objectService.GetObject<T>() : objectService.GetObject<T>(objectAlias);
			if (instance == null) {
				if (!aliasNull) {
					var type = objectService.GetOrCreateType(objectAlias);
					if (type != null) {
						instance = (T)objectService.CreateObject(type);
					}
				}
			}
			if (instance == null) {
				instance = objectService.CreateObject<T>();
			}
			return instance;
		}

		#endregion
	}
}
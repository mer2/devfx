/* Copyright(c) 2005-2011 R2@DevFx.NET, License(LGPL) */

using System;
using System.Collections.Generic;
using HTB.DevFx.Config;
using HTB.DevFx.Core.Config;
using HTB.DevFx.Esb.Config;
using HTB.DevFx.Exceptions;
using HTB.DevFx.Utils;
using HTB.DevFx.Core;

namespace HTB.DevFx.Esb
{
	/// <summary>
	/// 服务定位器，简单的IoC容器
	/// </summary>
	public abstract class ServiceLocator : IServiceLocator
	{
		#region instance members

		internal abstract T GetServiceInternal<T>();
		internal abstract T GetServiceInternal<T>(string serviceName);
		internal abstract object GetServiceInternal(string serviceName);
		internal abstract string GetTypeNameInternal(string typeAlias);

		/// <summary>
		/// 对象创建前事件
		/// </summary>
		public event Action<IObjectBuildContext> ObjectBuilding;
		/// <summary>
		/// 对象创建后事件
		/// </summary>
		public event Action<IObjectBuildContext> ObjectBuilt;

		#endregion
	
		#region IServiceLocator Members

		T IServiceLocator.GetService<T>() {
			return this.GetServiceInternal<T>();
		}

		T IServiceLocator.GetService<T>(string serviceName) {
			return this.GetServiceInternal<T>(serviceName);
		}

		object IServiceLocator.GetService(string serviceName) {
			return this.GetServiceInternal(serviceName);
		}

		string IServiceLocator.GetTypeName(string typeAlias) {
			return this.GetTypeNameInternal(typeAlias);
		}

		#endregion

		#region static members

		/// <summary>
		/// 初始化
		/// </summary>
		public static void Init() {
			Current.ToString();
		}

		/// <summary>
		/// 获得指定类型服务的实例
		/// </summary>
		/// <typeparam name="T">服务类型</typeparam>
		/// <returns>服务实例</returns>
		public static T GetService<T>() {
			return Current.GetService<T>();
		}

		/// <summary>
		/// 获得指定类型服务的实例
		/// </summary>
		/// <typeparam name="T">服务类型</typeparam>
		/// <param name="serviceName">实例名称</param>
		/// <returns>服务实例</returns>
		public static T GetService<T>(string serviceName) {
			return Current.GetService<T>(serviceName);
		}

		/// <summary>
		/// 获得指定类型服务的实例
		/// </summary>
		/// <param name="serviceName">实例名称</param>
		/// <returns>服务实例</returns>
		public static object GetService(string serviceName) {
			return Current.GetService(serviceName);
		}

		/// <summary>
		/// 获取类型别名对应的类型名
		/// </summary>
		/// <param name="typeAlias">别名</param>
		/// <returns>类型名</returns>
		public static string GetTypeName(string typeAlias) {
			return Current.GetTypeName(typeAlias);
		}

		/// <summary>
		/// 当前<see cref="IServiceLocator"/>实例
		/// </summary>
		public static IServiceLocator Current {
			get { return GetServiceLocator(); }
		}

		#endregion

		#region internal members

		private static IServiceLocator GetServiceLocator() {
			if (currentInstance == null) {
				lock (typeof(ServiceLocator)) {
					if (currentInstance == null) {
						if (currentInitializing) {
							throw new ExceptionBase(-1, "ServiceLocator正在初始化，不可用。");
						}
						currentInitializing = true;
						try {
							currentInstance = Setup();
						} finally {
							currentInitializing = false;
						}
					}
				}
			}
			return currentInstance;
		}

		private static IServiceLocator currentInstance;
		private static bool currentInitializing;
		private static IServiceLocator Setup() {
			var config = ConfigHelper.LoadConfigSettingFromAssemblies(typeof(ServiceLocator).Assembly, null, null, null);
			StartupSetting setting = null;
			if(config != null) {
				config = config["htb.devfx"];
				if (config != null) {
					setting = ConfigSetting.ToSetting<StartupSetting>(config, "startup");
				}
			}
			ServiceLocatorSetting locatorSetting = null;
			if(setting != null && setting.CoreSetting != null) {
				locatorSetting = setting.CoreSetting.ServiceLocatorSetting;
			}
			IServiceLocator instance = null;
			if(locatorSetting != null && !string.IsNullOrEmpty(locatorSetting.TypeName)) {
				var typeName = locatorSetting.TypeName;
				instance = (IServiceLocator)TypeHelper.CreateObject(typeName, typeof(IServiceLocator), false);
			}
			if(instance == null) {
				instance = new ServiceLocatorInternal();
			}
			if(instance is IServiceLocatorInternal) {
				((IServiceLocatorInternal)instance).Init(locatorSetting);
			}
			return instance;
		}

		internal abstract class ServiceLocatorBase : ServiceLocator, IServiceLocatorInternal
		{
			protected readonly List<IObjectExtender<IServiceLocator>> Extenders = new List<IObjectExtender<IServiceLocator>>();
			protected virtual void InitExtenders(ITypeSetting[] settings) {
				if (settings == null || settings.Length <= 0) {
					return;
				}
				foreach (var setting in settings) {
					var typeName = this.GetTypeNameInternal(setting.TypeName);
					var extender = (IObjectExtender<IServiceLocator>)TypeHelper.CreateObject(typeName, typeof(IObjectExtender<IServiceLocator>), true);
					if (extender != null) {
						extender.Init(this);
						this.Extenders.Add(extender);
					}
				}
			}

			protected virtual void OnObjectBuilding(ObjectBuildContext ctx) {
				if(this.ObjectBuilding != null){
					this.ObjectBuilding(ctx);
				}
			}

			protected virtual void OnObjectBuilt(ObjectBuildContext ctx) {
				if (this.ObjectBuilt != null) {
					this.ObjectBuilt(ctx);
				}
			}

			#region Implementation of IServiceLocatorInternal

			public virtual void Init(IServiceLocatorSetting setting) {
				if(setting != null) {
					this.InitExtenders(setting.Extenders);
				}
			}

			#endregion
		}

		internal class ServiceLocatorInternal : ServiceLocatorBase
		{
			private readonly Dictionary<string, string> typeAliases = new Dictionary<string, string>();
			private readonly Dictionary<Type, IObjectSettingInternal> typedObjects = new Dictionary<Type, IObjectSettingInternal>();
			private readonly Dictionary<string, IObjectSettingInternal> namedObjects = new Dictionary<string, IObjectSettingInternal>();

			public override void Init(IServiceLocatorSetting setting) {
				this.Init(setting as ServiceLocatorSetting);
				base.Init(setting);
			}

			internal void Init(ServiceLocatorSetting setting) {
				if(setting == null) {
					return;
				}
				if(setting.Container != null) {
					this.InitTypeAliases(setting.Container.TypeAliases);
					this.InitObjects(setting.Container.Objects);
				}
			}

			internal void InitTypeAliases(ITypeSetting[] typeAliases) {
				if(typeAliases == null || typeAliases.Length <= 0) {
					return;
				}
				foreach(var alias in typeAliases) {
					this.typeAliases.Add(alias.Name, alias.TypeName);
				}
			}

			internal void InitObjects(IObjectSettingInternal[] objectSettings) {
				if(objectSettings == null || objectSettings.Length <= 0) {
					return;
				}
				foreach(var setting in objectSettings) {
					this.namedObjects.Add(setting.Name, setting);
					var typeName = this.GetTypeNameInternal(setting.TypeName);
					var type = TypeHelper.CreateType(typeName, true);
					if(!this.typedObjects.ContainsKey(type)) {
						this.typedObjects.Add(type, setting);
					}
				}
			}

			private object GetObject(IObjectSetting setting, Type expectedType) {
				var builderName = setting.Builder;
				IObjectBuilder builder = null;
				if(!string.IsNullOrEmpty(builderName)) {
					builder = this.GetServiceInternal<IObjectBuilder>(builderName);
				}
				var ctx = new ObjectBuildContext(this, setting, builder, null);
				this.OnObjectBuilding(ctx);
				if(ctx.ObjectInstance == null && builder != null) {
					ctx.ObjectInstance = builder.CreateObject(setting);
				}
				if(ctx.ObjectInstance == null) {
					var mapTo = this.GetTypeNameInternal(setting.MapTo);
					if(!string.IsNullOrEmpty(mapTo)) {
						ctx.ObjectInstance = TypeHelper.CreateObject(mapTo, expectedType, true);
					} else {
						var typeName = this.GetTypeNameInternal(setting.TypeName);
						if(!string.IsNullOrEmpty(typeName)) {
							ctx.ObjectInstance = TypeHelper.CreateObject(typeName, expectedType, true);
						} else {
							throw new ConfigException("type attribute is required in <object />");
						}
					}
				}
				this.OnObjectBuilt(ctx);
				return ctx.ObjectInstance;
			}

			private object GetObjectSingleton(IObjectSettingInternal setting, Type expectedType) {
				if (setting.ObjectInstance == null) {
					lock (setting) {
						if (setting.ObjectInstance == null) {
							setting.ObjectInstance = this.GetObject(setting, expectedType);
						}
					}
				}
				return setting.ObjectInstance;
			}

			internal override T GetServiceInternal<T>() {
				var type = typeof(T);
				if(this.typedObjects.ContainsKey(type)) {
					var setting = this.typedObjects[type];
					return (T)this.GetObjectSingleton(setting, type);
				}
				return default(T);
			}

			internal string GetObjectName(string objectAlias) {
				if(!string.IsNullOrEmpty(objectAlias) && objectAlias.StartsWith("@")) {
					return objectAlias.Substring(1);
				}
				return objectAlias;
			}

			internal override T GetServiceInternal<T>(string serviceName) {
				serviceName = this.GetObjectName(serviceName);
				if(this.namedObjects.ContainsKey(serviceName)) {
					var setting = this.namedObjects[serviceName];
					return (T)this.GetObjectSingleton(setting, typeof(T));
				}
				return default(T);
			}

			internal override object GetServiceInternal(string serviceName) {
				serviceName = this.GetObjectName(serviceName);
				if(this.namedObjects.ContainsKey(serviceName)) {
					var setting = this.namedObjects[serviceName];
					return this.GetObjectSingleton(setting, null);
				}
				return null;
			}

			internal override string GetTypeNameInternal(string typeAlias) {
				if(string.IsNullOrEmpty(typeAlias)) {
					return typeAlias;
				}
				string typeName;
				if (!this.typeAliases.TryGetValue(typeAlias, out typeName)) {
					typeName = typeAlias;
				}
				return typeName;
			}
		}

		#endregion
	}
}
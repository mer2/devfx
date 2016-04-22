/* Copyright(c) 2005-2011 R2@DevFx.NET, License(LGPL) */

using System;
using System.Collections;
using System.Collections.Generic;
using HTB.DevFx.Config;
using HTB.DevFx.Core.Config;
using HTB.DevFx.Core.Lifetime;
using HTB.DevFx.Utils;

namespace HTB.DevFx.Core
{
	public abstract partial class ObjectServiceBase : ServiceBase, IDisposable
	{
		#region Init

		protected override void OnInit() {
			this.objectNamespaces = new Dictionary<string, ObjectNamespace>();
			this.globalObjectNamespace = new ObjectNamespace(GlobalObjectNamespaceName);
			this.objectNamespaces.Add(GlobalObjectNamespaceName, this.globalObjectNamespace);

			this.ObjectBuilder = new ObjectBuilderInternal(this);

			ILifetimeContainer configServiceLifetime = new SingletonLifetimeContainer();
			configServiceLifetime.Init(null, this.ObjectBuilder);
			configServiceLifetime.SetObject(this.ConfigService);
			var configServiceType = typeof(IConfigService);
			this.globalObjectNamespace.TypedObjects.Add(configServiceType, configServiceLifetime);

			ILifetimeContainer objectServiceLifetime = new SingletonLifetimeContainer();
			objectServiceLifetime.Init(null, this.ObjectBuilder);
			objectServiceLifetime.SetObject(this);
			var objectServiceType = typeof(IObjectService);
			this.globalObjectNamespace.TypedObjects.Add(objectServiceType, objectServiceLifetime);

			IStartupSetting setting = this.ConfigService.ToSetting<IObjectService, StartupSetting>();
			if (setting == null || setting.CoreSetting == null || setting.CoreSetting.ObjectServiceSetting == null) {
				return;
			}
			var objectServiceSetting = setting.CoreSetting.ObjectServiceSetting;
			if(objectServiceSetting.Extenders != null && objectServiceSetting.Extenders.Length > 0) {
				foreach(var extenderSetting in objectServiceSetting.Extenders) {
					if(!extenderSetting.Enabled) {
						continue;
					}
					var extender = (IObjectExtender<IObjectService>)TypeHelper.CreateObject(extenderSetting.TypeName, typeof(IObjectExtender<IObjectService>), false);
					if(extender != null) {
						extender.Init(this);
					}
				}
			}

			this.OnPreInit();
			if(objectServiceSetting.ObjectNamespace == null) {
				return;
			}
			var spaceSetting = objectServiceSetting.ObjectNamespace;
			var spaceSettings = new Dictionary<ObjectNamespace, IObjectNamespaceSetting> {
				{ this.globalObjectNamespace, spaceSetting }
			};
			this.InitNamespace(spaceSetting, GlobalObjectNamespaceName, spaceSettings);
			this.InitTypeAliases(spaceSettings);
			this.InitConstAliases(spaceSettings);
			this.InitObjectAliases(spaceSettings, x => {
				if (objectServiceType.IsAssignableFrom(x)) {
					return objectServiceLifetime;
				}
				if (configServiceType.IsAssignableFrom(x)) {
					return configServiceLifetime;
				}
				return null;
			});
		}
		
		internal virtual void InitNamespace(IObjectNamespaceSetting spaceSetting, string parentName, Dictionary<ObjectNamespace, IObjectNamespaceSetting> list) {
			var spaceName = spaceSetting.Name;
			if(!string.IsNullOrEmpty(parentName)) {
				spaceName = parentName + "." + spaceName;
			}
			if (!this.objectNamespaces.ContainsKey(spaceName)) {
				var space = new ObjectNamespace(spaceName);
				this.objectNamespaces.Add(spaceName, space);
				list.Add(space, spaceSetting);
			}
			var spaceSettings = spaceSetting.ObjectNamespaces;
			if(spaceSettings == null || spaceSettings.Length <= 0) {
				return;
			}
			foreach(var setting in spaceSettings) {
				this.InitNamespace(setting, spaceName, list);
			}
		}

		internal virtual void InitTypeAliases(Dictionary<ObjectNamespace, IObjectNamespaceSetting> list) {
			foreach (var space in list.Keys) {
				var setting = list[space];
				this.InitTypeAliases(space, setting);
			}
		}

		internal virtual void InitTypeAliases(ObjectNamespace space, IObjectNamespaceSetting setting) {
			if (setting.TypeAliases == null || setting.TypeAliases.Length <= 0) {
				return;
			}

			var aliases = space.TypeAliases;
			var aliasesGlobal = this.globalObjectNamespace.TypeAliases;

			var preName = space.Name;
			if (!string.IsNullOrEmpty(preName)) {
				preName += ".";
			}

			foreach (var typeSetting in setting.TypeAliases) {
				var name = typeSetting.Name;
				var value = typeSetting.TypeName;
				aliases.Add(name, value);
				if (space != this.globalObjectNamespace) {
					aliasesGlobal.Add(preName + name, value);
				}
			}
		}

		internal virtual void InitConstAliases(Dictionary<ObjectNamespace, IObjectNamespaceSetting> list) {
			foreach (var space in list.Keys) {
				var setting = list[space];
				this.InitConstAliases(space, setting);
			}
		}

		internal virtual void InitConstAliases(ObjectNamespace space, IObjectNamespaceSetting setting) {
			if (setting.ConstAliases == null || setting.ConstAliases.Length <= 0) {
				return;
			}

			var aliases = space.ConstAliases;
			var aliasesGlobal = this.globalObjectNamespace.ConstAliases;

			var spaceName = space.Name;
			var preName = spaceName;
			if (!string.IsNullOrEmpty(preName)) {
				preName += ".";
			}

			foreach (var constSetting in setting.ConstAliases) {
				var name = constSetting.Name;
				var value = Convert.ChangeType(constSetting.Value, TypeHelper.CreateType(this.GetTypeNameInternal(constSetting.TypeName, spaceName), true));
				aliases.Add(name, value);
				if (space != this.globalObjectNamespace) {
					aliasesGlobal.Add(preName + name, value);
				}
			}
		}

		internal virtual void InitObjectAliases(Dictionary<ObjectNamespace, IObjectNamespaceSetting> list, Func<Type, ILifetimeContainer> predicate) {
			foreach (var space in list.Keys) {
				var setting = list[space];
				this.InitObjectAliases(space, setting, predicate);
			}
		}

		internal virtual void InitObjectAliases(ObjectNamespace space, IObjectNamespaceSetting spaceSetting, Func<Type, ILifetimeContainer> predicate) {
			if(spaceSetting.ObjectSettings == null || spaceSetting.ObjectSettings.Length <= 0) {
				return;
			}

			var spaceName = space.Name;
			var preName = spaceName;
			if (!string.IsNullOrEmpty(preName)) {
				preName += ".";
			}

			foreach (var objectSetting in spaceSetting.ObjectSettings) {
				var objectTypeName = this.GetTypeNameInternal(objectSetting.TypeName, spaceName);
				var objectType = TypeHelper.CreateType(objectTypeName, true);
				var objectName = objectSetting.Name;
				var container = predicate(objectType);
				if (container == null) {
					if (objectSetting.Lifetime != null && !string.IsNullOrEmpty(objectSetting.Lifetime.TypeName)) {
						var lifetimeContainerTypeName = this.GetTypeNameInternal(objectSetting.Lifetime.TypeName, spaceName);
						container = (ILifetimeContainer)TypeHelper.CreateObject(lifetimeContainerTypeName, typeof(ILifetimeContainer), true);
					} else {
						container = LifetimeContainer.CreateDefault();
					}
					this.globalObjectNamespace.TypedObjects.Add(objectType, container);
				}
				space.TypedObjects.Add(objectType, container);
				space.ObjectAliases.Add(objectName, container);
				if (this.globalObjectNamespace != space) {
					this.globalObjectNamespace.ObjectAliases.Add(preName + objectName, container);
				}

				container.Init(objectSetting, this.ObjectBuilder);
			}
		}

		internal virtual void InitCompletedInternal() {
			foreach (var container in this.globalObjectNamespace.ObjectAliases.Values) {
				container.InitCompleted();
			}
			this.OnInitCompleted();
		}

		#endregion

		#region GetLifetimeContainer

		protected virtual ILifetimeContainer GetLifetimeContainer(Type objectKey) {
			ILifetimeContainer lifetimeContainer = null;
			if(this.globalObjectNamespace.TypedObjects.ContainsKey(objectKey)) {
				var typedObject = this.globalObjectNamespace.TypedObjects[objectKey];
				if(typedObject != null) {
					lifetimeContainer = typedObject.Container;
				}
			}
			return lifetimeContainer;
		}

		protected virtual ILifetimeContainer GetLifetimeContainer(string objectKey) {
			return this.GetLifetimeContainer(objectKey, GlobalObjectNamespaceName);
		}

		protected virtual ILifetimeContainer GetLifetimeContainer(string objectKey, string spaceName) {
			ILifetimeContainer lifetimeContainer = null;
			if(!string.IsNullOrEmpty(spaceName) && this.objectNamespaces.ContainsKey(spaceName)) {
				this.objectNamespaces[spaceName].ObjectAliases.TryGetValue(objectKey, out lifetimeContainer);
			}
			if (lifetimeContainer == null) {
				this.globalObjectNamespace.ObjectAliases.TryGetValue(objectKey, out lifetimeContainer);
			}
			return lifetimeContainer;
		}

		protected internal virtual ILifetimeContainer GetLifetimeContainer(object objectKey) {
			return this.GetLifetimeContainer(objectKey, GlobalObjectNamespaceName);
		}

		protected internal virtual ILifetimeContainer GetLifetimeContainer(object objectKey, string spaceName) {
			if (objectKey is Type) {
				return this.GetLifetimeContainer((Type)objectKey);
			}
			return this.GetLifetimeContainer(objectKey.ToString(), spaceName);
		}

		#endregion GetLifetimeContainer

		#region GetObjectInternal
		
		protected virtual T GetObjectInternal<T>(IDictionary items) {
			return (T)this.GetObjectInternal(typeof(T), default(T), items);
		}

		protected virtual object GetObjectInternal(Type type, object defaultValue, IDictionary items) {
			if(typeof(IObjectService).IsAssignableFrom(type) || typeof(IObjectServiceInternal).IsAssignableFrom(type)) {
				return this;
			}
			if (typeof(IConfigService).IsAssignableFrom(type) || typeof(IConfigServiceInternal).IsAssignableFrom(type)) {
				return this.ConfigService;
			}

			var lifetimeContainer = this.GetLifetimeContainer(type);
			if (lifetimeContainer != null) {
				return lifetimeContainer.GetObject(items);
			}
			return defaultValue;
		}

		protected virtual object GetObjectInternal(string objectAlias, IDictionary items) {
			return this.GetObjectInternal(objectAlias, GlobalObjectNamespaceName, true, items);
		}

		protected virtual object GetObjectInternal(string objectAlias, string spaceName, bool aliasFlag, IDictionary items) {
			if (aliasFlag) {
				this.GetObjectAlias(ref objectAlias);
			}
			object value = null;
			if (!string.IsNullOrEmpty(spaceName)) {
				if(this.objectNamespaces.ContainsKey(spaceName)) {
					this.objectNamespaces[spaceName].ConstAliases.TryGetValue(objectAlias, out value);
				}
				if(value == null) {
					var container = this.GetLifetimeContainer(objectAlias, spaceName);
					if (container != null) {
						value = container.GetObject(items);
					}
				}
			}
			if (value == null) {
				this.globalObjectNamespace.ConstAliases.TryGetValue(objectAlias, out value);
			}
			if (value == null) {
				var container = this.GetLifetimeContainer(objectAlias);
				if (container != null) {
					value = container.GetObject(items);
				}
			}
			return value;
		}

		protected internal virtual object GetObjectInternal(string objectAlias, string spaceName, Type expectedType, IDictionary items) {
			var isObjectAlias = this.GetObjectAlias(ref objectAlias);
			object value = null;
			if (isObjectAlias) {
				value = this.GetObjectInternal(objectAlias, spaceName, false, items);
			} else {
				if(expectedType != null) {
					if(expectedType == typeof(string)) {
						value = objectAlias;
					} else if(expectedType == typeof(Type)) {
						value = TypeHelper.CreateType(objectAlias, expectedType, true);
					} else if(expectedType.IsEnum) {
						value = Enum.Parse(expectedType, objectAlias, true);
					} else if (expectedType == typeof(TimeSpan)) {
						value = TimeSpan.Parse(objectAlias);
					} else if (expectedType == typeof(Guid)) {
						value = new Guid(objectAlias);
					} else {
						value = Convert.ChangeType(objectAlias, expectedType);
					}
				}
			}
			return value;
		}

		protected internal virtual string GetObjectAlias(string objectAlias) {
			this.GetObjectAlias(ref objectAlias);
			return objectAlias;
		}

		protected internal virtual bool GetObjectAlias(ref string objectAlias) {
			var isObjectAlias = false;
			if (objectAlias != null) {
				if(objectAlias.StartsWith("@@")) {
					objectAlias = objectAlias.Substring(1);
				} else if(objectAlias.StartsWith("@")) {
					objectAlias = objectAlias.Substring(1);
					isObjectAlias = true;
				}
			}
			return isObjectAlias;
		}

		protected virtual object[] GetObjectsInternal(Type objectType, IDictionary items) {
			var values = new List<object>();
			TypedObject typedObject;
			if(this.globalObjectNamespace.TypedObjects.TryGetValue(objectType, out typedObject)) {
				foreach(var container in typedObject.Containers) {
					values.Add(container.GetObject(items));
				}
			}
			if(values.Count > 0) {
				var returnValues = Array.CreateInstance(objectType, values.Count);
				for (var i = 0; i < values.Count; i++) {
					returnValues.SetValue(values[i], i);
				}
				return (object[])returnValues;
			}
			return null;
		}

		protected virtual T[] GetObjectsInternal<T>(IDictionary items) {
			return this.GetObjectsInternal(typeof(T), items) as T[];
		}

		#endregion

		#region CreateObject

		protected virtual T CreateObjectInternal<T>(IDictionary items, params object[] args) {
			return (T)this.CreateObjectInternal(typeof(T), default(T), items, args);
		}

		protected virtual object CreateObjectInternal(Type type, object defaultValue, IDictionary items, params object[] args) {
			var lifetimeContainer = this.GetLifetimeContainer(type);
			if (lifetimeContainer != null) {
				return this.CreateObjectInternal(lifetimeContainer.ObjectSetting, items, args);
			}
			return defaultValue;
		}

		protected virtual object CreateObjectInternal(string objectAlias, object defaultValue, IDictionary items, params object[] args) {
			this.GetObjectAlias(ref objectAlias);
			var lifetimeContainer = this.GetLifetimeContainer(objectAlias);
			if (lifetimeContainer != null) {
				return this.CreateObjectInternal(lifetimeContainer.ObjectSetting, items, args);
			}
			return defaultValue;
		}

		protected virtual object CreateObjectInternal(IObjectSetting objectSetting, IDictionary items, params object[] args) {
			return this.ObjectBuilder.CreateObject(objectSetting, items, args);
		}

		protected virtual object CreateObjectInternal(IObjectSetting objectSetting, IDictionary items) {
			return this.ObjectBuilder.CreateObject(objectSetting, items);
		}

		#endregion

		#region GetTypeName

		protected internal virtual string GetTypeNameInternal(string typeAlias) {
			return this.GetTypeNameInternal(typeAlias, GlobalObjectNamespaceName);
		}

		protected internal virtual string GetTypeNameInternal(string typeAlias, string spaceName) {
			string typeName = null;
			if(!string.IsNullOrEmpty(spaceName) && this.objectNamespaces.ContainsKey(spaceName)) {
				this.objectNamespaces[spaceName].TypeAliases.TryGetValue(typeAlias, out typeName);
			}
			if (typeName == null && !this.globalObjectNamespace.TypeAliases.TryGetValue(typeAlias, out typeName)) {
				typeName = typeAlias;
			}
			return typeName;
		}

		#endregion

		#region GetObjectSetting

		protected virtual IConfigSetting GetObjectSettingInternal(object objectKey) {
			var lifetimeContainer = this.GetLifetimeContainer(objectKey);
			if (lifetimeContainer != null) {
				var objectSetting = lifetimeContainer.ObjectSetting;
				return this.GetObjectSettingInternal(objectSetting);
			}
			return null;
		}

		protected internal virtual IConfigSetting GetObjectSettingInternal(IObjectSetting objectSetting) {
			if (objectSetting != null) {
				var settingSet = objectSetting.SettingSet;
				var settingName = settingSet != null ? settingSet.Name : ObjectSettingConfigPropertyName;
				return objectSetting.ConfigSetting.GetChildSetting(settingName);
			}
			return null;
		}

		protected virtual IObjectSetting GetObjectBuildingSettingInternal(object objectKey) {
			return this.GetObjectBuildingSettingInternal(objectKey, GlobalObjectNamespaceName);
		}

		protected virtual IObjectSetting GetObjectBuildingSettingInternal(object objectKey, string spaceName) {
			if(objectKey is string) {
				objectKey = this.GetObjectAlias((string)objectKey);
			}
			var lifetimeContainer = this.GetLifetimeContainer(objectKey, spaceName);
			return lifetimeContainer != null ? lifetimeContainer.ObjectSetting : null;
		}

		#endregion
	
		#region Properties and fields

		protected override IObjectService ObjectService {
			get { return this; }
		}

		protected override IConfigService ConfigService {
			get { return this.currentConfigService; }
		}

		protected virtual IObjectBuilder ObjectBuilder { get; private set; }
		protected internal const string GlobalObjectNamespaceName = "";
		protected internal const string ObjectSettingConfigPropertyName = "setting";

		private Dictionary<string, ObjectNamespace> objectNamespaces;
		private ObjectNamespace globalObjectNamespace;
		private IConfigService currentConfigService;

		#endregion Properties and fields

		#region IDisposable Members

		private bool disposed;
		protected virtual void Dispose() {
			if(!this.disposed) {
				this.OnDisposing();
				this.disposed = true;
			}
		}

		void IDisposable.Dispose() {
			this.Dispose();
			GC.SuppressFinalize(this);
		}

		~ObjectServiceBase() {
			this.Dispose();
		}

		#endregion
	}
}
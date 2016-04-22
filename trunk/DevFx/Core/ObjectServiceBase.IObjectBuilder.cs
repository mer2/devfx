/* Copyright(c) 2005-2011 R2@DevFx.NET, License(LGPL) */

using System;
using System.Collections;
using System.Collections.Generic;
using HTB.DevFx.Config;
using HTB.DevFx.Core.Config;
using HTB.DevFx.Reflection;
using HTB.DevFx.Utils;

namespace HTB.DevFx.Core
{
	partial class ObjectServiceBase
	{
		internal class ObjectBuilderInternal : IObjectBuilder
		{
			public ObjectBuilderInternal(ObjectServiceBase objectService) {
				this.ObjectService = objectService;
				this.ObjectSettingDependencies = new Dictionary<IObjectSetting, bool>();
			}

			protected event Action<IObjectBuilderContext> ObjectCreated;
			protected event Action<IObjectBuilderContext> ObjectCreating;
			protected virtual ObjectServiceBase ObjectService { get; private set; }
			protected virtual Dictionary<IObjectSetting, bool> ObjectSettingDependencies { get; private set; }

			protected virtual object OnObjectCreated(IObjectBuilderContext ctx) {
				if (this.ObjectCreated != null) {
					this.ObjectCreated(ctx);
				}
				return ctx.ObjectInstance;
			}

			protected virtual object OnObjectCreating(IObjectBuilderContext ctx) {
				if (this.ObjectCreating != null) {
					this.ObjectCreating(ctx);
				}
				return ctx.ObjectInstance;
			}

			protected virtual object CreateObject(IObjectSetting objectSetting, IDictionary items) {
				var ctx = new ObjectBuilderContext(this, objectSetting, items);
				var instance = this.OnObjectCreating(ctx);
				if(instance != null) {
					return instance;
				}

				this.CheckObjectSettingDependencies(objectSetting);

				var builderType = objectSetting.Builder;
				if (!string.IsNullOrEmpty(builderType)) {
					var builder = (Esb.IObjectBuilder)this.ObjectService.GetObjectInternal(builderType, objectSetting.Namespace, typeof(Esb.IObjectBuilder), ctx.Items);
					if (builder != null) {
						instance = builder.CreateObject(objectSetting);
						if(instance != null) {
							goto EXIT;
						}
					}
				}

				var spaceName = objectSetting.Namespace;
				if (objectSetting.Dependencies != null && objectSetting.Dependencies.Length > 0) {
					foreach (var dependencySetting in objectSetting.Dependencies) {
						var value = dependencySetting.Name;
						this.ObjectService.GetObjectInternal(value, spaceName, null, items);
					}
				}

				var mapTo = objectSetting.MapTo;
				if (string.IsNullOrEmpty(mapTo)) {
					mapTo = objectSetting.TypeName;
				} else {
					instance = this.ObjectService.GetObjectInternal(mapTo, spaceName, null, items);
					if(instance != null) {
						return instance;
					}
				}
				mapTo = this.ObjectService.GetTypeNameInternal(mapTo, spaceName);

				var constructor = objectSetting.Constructor;
				if (constructor != null) {
					var parameters = constructor.Parameters;
					if(parameters != null) {
						var length = parameters.Length;
						if(length > 0) {
							var types = new Type[length];
							var values = new object[length];
							for(var i = 0; i < parameters.Length; i++) {
								var parameter = parameters[i];
								var typeName = this.ObjectService.GetTypeNameInternal(parameter.TypeName, spaceName);
								var type = TypeHelper.CreateType(typeName, true);
								types[i] = type;
								var valueString = parameter.Value;
								var value = this.ObjectService.GetObjectInternal(valueString, spaceName, type, items);
								values[i] = value;
							}
							instance = TypeHelper.CreateObject(mapTo, null, true, types, values);
						}
					}
				}
	
				if(instance == null) {
					instance = TypeHelper.CreateObject(mapTo, null, true);
				}

				if(instance != null && objectSetting.Properties != null && objectSetting.Properties.Length > 0) {
					foreach (var property in objectSetting.Properties) {
						var typeName = this.ObjectService.GetTypeNameInternal(property.TypeName, spaceName);
						var type = TypeHelper.CreateType(typeName, true);
						var propertyName = property.Name;
						var valueName = property.Value;
						var value = this.ObjectService.GetObjectInternal(valueName, spaceName, type, items);
						if (instance is IPropertyDescriptor) {
							((IPropertyDescriptor)instance).SetValue(propertyName, value);
						} else {
							TypeHelper.SetPropertyValue(instance, propertyName, value);
						}
					}
				}

				EXIT:

				if(instance != null) {
					ctx.ObjectInstance = instance;
					this.OnObjectCreated(ctx);
				}

				return instance;
			}

			protected virtual object CreateObject(IObjectSetting objectSetting, IDictionary items, params object[] args) {
				var ctx = new ObjectBuilderContext(this, objectSetting, items);
				var instance = this.OnObjectCreating(ctx);
				if (instance != null) {
					return instance;
				}

				var mapTo = objectSetting.MapTo;
				if (string.IsNullOrEmpty(mapTo)) {
					mapTo = objectSetting.TypeName;
				}
				mapTo = this.ObjectService.GetTypeNameInternal(mapTo, objectSetting.Namespace);
				instance = TypeHelper.CreateObject(mapTo, null, true, args);
				if(instance != null) {
					ctx.ObjectInstance = instance;
					this.OnObjectCreated(ctx);
				}
				return instance;
			}

			protected virtual void CheckObjectSettingDependencies(IObjectSetting objectSetting) {
				bool state;
				if(!this.ObjectSettingDependencies.TryGetValue(objectSetting, out state)) {
					lock (this.ObjectSettingDependencies) {
						if(!this.ObjectSettingDependencies.TryGetValue(objectSetting, out state)) {
							state = this.CheckObjectSettingDependencies(objectSetting, new List<IObjectSetting>());
						}
					}
				}
				if(!state) {
					throw new ConfigException("对象依赖进入死循环，请检查配置");
				}
			}

			protected virtual bool CheckObjectSettingDependencies(IObjectSetting objectSetting, List<IObjectSetting> settingRoute) {
				if(this.ObjectSettingDependencies.ContainsKey(objectSetting)) {
					return this.ObjectSettingDependencies[objectSetting];
				}

				var state = true;
				var currentRoute = new List<IObjectSetting>(settingRoute) { objectSetting };
				var currentDependencies = new List<IObjectSetting>();

				var spaceName = objectSetting.Namespace;
				var builderType = objectSetting.Builder;
				if(!string.IsNullOrEmpty(builderType)) {
					var setting = this.ObjectService.GetObjectBuildingSettingInternal(builderType, spaceName);
					if (setting != null && !currentDependencies.Contains(setting)) {
						if (currentRoute.Contains(setting)) {
							state = false;
							goto EXIT;
						}
						currentDependencies.Add(setting);
					}
				}

				if(objectSetting.Properties != null && objectSetting.Properties.Length > 0) {
					foreach(var property in objectSetting.Properties) {
						var value = property.Value;
						var setting = this.ObjectService.GetObjectBuildingSettingInternal(value, spaceName);
						if (setting != null && !currentDependencies.Contains(setting)) {
							if (currentRoute.Contains(setting)) {
								state = false;
								goto EXIT;
							}
							currentDependencies.Add(setting);
						}
					}
				}

				if(objectSetting.Dependencies != null && objectSetting.Dependencies.Length > 0) {
					foreach (var dependencySetting in objectSetting.Dependencies) {
						var value = dependencySetting.Name;
						var setting = this.ObjectService.GetObjectBuildingSettingInternal(value, spaceName);
						if (setting != null && !currentDependencies.Contains(setting)) {
							if (currentRoute.Contains(setting)) {
								state = false;
								goto EXIT;
							}
							currentDependencies.Add(setting);
						}
					}
				}
	
				var constructor = objectSetting.Constructor;
				if(constructor != null) {
					var parameters = constructor.Parameters;
					if (parameters != null) {
						var length = parameters.Length;
						if (length > 0) {
							for (var i = 0; i < parameters.Length; i++) {
								var parameter = parameters[i];
								var value = parameter.Value;
								var setting = this.ObjectService.GetObjectBuildingSettingInternal(value, spaceName);
								if (setting != null && !currentDependencies.Contains(setting)) {
									if(currentRoute.Contains(setting)) {
										state = false;
										goto EXIT;
									}
									currentDependencies.Add(setting);
								}
							}
						}
					}
				}

				foreach (var setting in currentDependencies) {
					var settingDependencies = new List<IObjectSetting>(currentRoute);
					if (!this.CheckObjectSettingDependencies(setting, settingDependencies)) {
						state = false;
						goto EXIT;
					}
				}

				EXIT:
				this.ObjectSettingDependencies.Add(objectSetting, state);
				return state;
			}

			#region IObjectBuilder Members

			object IObjectBuilder.CreateObject(IObjectSetting objectSetting) {
				return this.CreateObject(objectSetting, null);
			}

			object IObjectBuilder.CreateObject(IObjectSetting objectSetting, params object[] args) {
				return this.CreateObject(objectSetting, null, args);
			}

			object IObjectBuilder.CreateObject(IObjectSetting objectSetting, IDictionary items) {
				return this.CreateObject(objectSetting, items);
			}

			object IObjectBuilder.CreateObject(IObjectSetting objectSetting, IDictionary items, params object[] args) {
				return this.CreateObject(objectSetting, items, args);
			}

			event Action<IObjectBuilderContext> IObjectBuilder.ObjectCreating {
				add { this.ObjectCreating += value; }
				remove { this.ObjectCreating -= value; }
			}

			event Action<IObjectBuilderContext> IObjectBuilder.ObjectCreated {
				add { this.ObjectCreated += value; }
				remove { this.ObjectCreated -= value; }
			}

			#endregion
		}
	}
}

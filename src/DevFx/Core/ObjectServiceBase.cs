/* Copyright(c) 2005-2011 R2@DevFx.NET, License(LGPL) */

using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using DevFx.Configuration;
using DevFx.Core.Lifetime;
using DevFx.Core.Settings;
using DevFx.Utils;

namespace DevFx.Core
{
	public abstract partial class ObjectServiceBase
	{
		#region Init

		private bool initialized;
		internal void Init(IConfigSetting configSetting, IDictionary items) {
			if (!this.initialized) {
				lock (this) {
					if (!this.initialized) {
						this.OnInit(configSetting, items);
						this.initialized = true;
					}
				}
			}
		}

		protected void OnInit(IConfigSetting configSetting, IDictionary items) {
			this.ConfigSetting = configSetting;

			//定义全局容器，所有的对象/类型均会存在此容器中
			this.globalObjectNamespace = new ObjectNamespace(GlobalObjectNamespaceName);

			//把自己IObjectService添加到全局容器中
			var objectServiceType = typeof(IObjectService);
			var objectServiceLifetime = AddSingletonObjectToObjectNamespace(null, objectServiceType, this, this.globalObjectNamespace);

			//从程序集扫描定义了CoreAttribute的类型，缓存以供后续处理
			//Key为CoreAttribute派生类型，Value为CoreAttribute派生类型实例列表
			var attributes = this.CoreAttributes = new Dictionary<Type, IList<CoreAttribute>>();
			ScanCoreAttributes(attributes);

			//处理RemoveAttribute
			var removeAttributes = TypeHelper.GetAttributeFromAssembly<RemoveAttribute>(null);
			if(removeAttributes != null && removeAttributes.Length > 0) {
				foreach(var removeAttribute in removeAttributes) {
					attributes.TryGetValue(removeAttribute.AttributeType, out var list);
					if(list != null && list.Count > 0) {
						var ilist = (List<CoreAttribute>)list;
						if(!string.IsNullOrEmpty(removeAttribute.Name)) {
							ilist.RemoveAll(x => x.Name == removeAttribute.Name || x?.OwnerType.FullName == removeAttribute.Name);
						}
						if(removeAttribute.OwnerType != null) {
							ilist.RemoveAll(x => x.OwnerType == removeAttribute.OwnerType);
						}
					}
				}
			}

			//创建默认的对象创建器
			var objectBuilder = new ObjectBuilderInternal(this);
			this.ObjectBuilder = objectBuilder;
			//把对象创建器添加到全局容器中
			AddSingletonObjectToObjectNamespace(null, typeof(IObjectBuilder), objectBuilder, this.globalObjectNamespace);

			//创建初始化时的上下文
			items = items ?? new HybridDictionary();
			var ctx = new ObjectServiceContext(this, items) {
				GlobalObjectNamespace = this.globalObjectNamespace,
				CoreAttributes = attributes,
				ConfigSetting = configSetting
			};

			//增加对异常的处理
			AppDomain.CurrentDomain.UnhandledException += (sender, e) => {
				this.OnError(ctx, e.ExceptionObject as Exception);
				if(e.IsTerminating) {
					this.OnDisposing(ctx);
				}
			};

			//增加对卸载的处理
			AppDomain.CurrentDomain.ProcessExit += (sender, e) => {
				this.OnDisposing(ctx);
			};

			//从缓存中处理ObjectService的扩展器
			ExtenderAttributesHandle(ctx);
			//构建IObjectBuilder对象
			ObjectBuilderAttributeHandle(ctx);
			//发出即将初始化的消息
			this.OnPreInit(ctx);
			//从缓存中构建类型别名
			TypeAliaseAttributesHandle(ctx);
			//从缓存中构建对象容器内容
			ObjectAttributesHandle(ctx);

			//从文件配置里构建内容
			var setting = this.ConfigSetting.ToSetting<ObjectServiceSetting>(ConfigHelper.RootSettingName);
			var container = setting?.Container;
			if(container != null) {
				//初始化类型别名
				InitTypeAliases(this.globalObjectNamespace, container);
				//初始化常量
				this.InitConstAliases(this.globalObjectNamespace, container);
				//初始化对象
				this.InitObjectAliases(this.globalObjectNamespace, container, x => {
					if (objectServiceType.IsAssignableFrom(x)) {//保证IObjectService实例唯一
						return objectServiceLifetime;
					}
					return null;
				});
			}

			//处理CoreAttribute
			CoreAttributeHandle(ctx);
		}

		//从程序集扫描定义了CoreAttribute的类型，并缓存以供后续处理
		private static void ScanCoreAttributes(IDictionary<Type, IList<CoreAttribute>> dict) {
			var assemblies = TypeHelper.FindAssemblyChildren();//仅处理有引用本类库的程序集/或处理本类库

			var theAssembly = typeof(ObjectService).Assembly;
			var theName = theAssembly.FullName;
			Task.Factory.StartNew(() => {
				assemblies.ForEach(ass => {
					//处理定义在Assembly上的属性
					var assAttributes = ass.GetCustomAttributes<CoreAttribute>();
					AddAttributesToDictionary(null, assAttributes, dict);
					//处理定义在类型上的属性
					var types = ass.GetTypes();
					foreach(var type in types) {
						if (type.IsDefined(typeof(CoreAttribute), true)) {
							var attributes = type.GetCustomAttributes<CoreAttribute>();
							AddAttributesToDictionary(type, attributes, dict);
						}
					}
				});
			}).Wait();
		}

		private static void AddAttributesToDictionary(Type ownerType, IEnumerable<CoreAttribute> attributes, IDictionary<Type, IList<CoreAttribute>> dict) {
			if (attributes == null || attributes.Count() <= 0) {
				return;
			}
			foreach (var attribute in attributes) {
				if(ownerType != null) {
					attribute.OwnerType = ownerType;
				}
				var attributeType = attribute.GetType();
				if (!dict.TryGetValue(attributeType, out var attributeList)) {
					attributeList = new List<CoreAttribute>();
					dict.Add(attributeType, attributeList);
				}
				attributeList.Add(attribute);
			}
		}

		//处理定义在类上的扩展器
		private static void ExtenderAttributesHandle(IObjectServiceContext ctx) {
			ObjectServiceExtenderAttribute.ExtenderInit(ctx);
		}

		//构建IObjectBuilder对象
		private static void ObjectBuilderAttributeHandle(IObjectServiceContext ctx) {
			var attributes = ctx.CoreAttributes;
			if (!attributes.TryGetValue(typeof(ObjectBuilderAttribute), out var list)) {
				return;
			}
			foreach (var coreAttribute in list) {
				var attribute = (ObjectBuilderAttribute)coreAttribute;
				var type = attribute?.OwnerType;
				if (type != null) {
					var builder = (IObjectBuilder)TypeHelper.CreateObject(type, typeof(IObjectBuilder), false, ctx.ObjectService);
					if(builder != null) {
						AddSingletonObjectToObjectNamespace(attribute.Name, null, builder, ctx.GlobalObjectNamespace);
					}
				}
			}
		}

		private static IObjectContainer AddSingletonObjectToObjectNamespace(string name, Type type, object instance, IObjectNamespace objectNamespace) {
			IObjectContainer container = new SingletonObjectContainer();
			container.Init(null, null);
			container.SetObject(instance, null);
			if(!string.IsNullOrEmpty(name)) {
				objectNamespace.AddObject(name, container);
			}
			if(type != null) {
				objectNamespace.AddObject(type, container);
			}
			return container;
		}

		//构建类型别名
		private static void TypeAliaseAttributesHandle(IObjectServiceContext ctx) {
			var attributes = ctx.CoreAttributes;
			if(!attributes.TryGetValue(typeof(TypeAliaseAttribute), out var list)) {
				return;
			}
			var typeAliases = ctx.GlobalObjectNamespace.TypeAliases;
			foreach(var coreAttribute in list) {
				var attribute = (TypeAliaseAttribute)coreAttribute;
				if(attribute.OwnerType != null) {
					typeAliases.Add(attribute.Name, attribute.OwnerType);
				}
			}
		}

		//构建对象容器内容
		private static void ObjectAttributesHandle(IObjectServiceContext ctx) {
			var attributes = ctx.CoreAttributes;
			if(!attributes.TryGetValue(typeof(ObjectAttribute), out var list)) {
				return;
			}
			foreach(var coreAttribute in list) {
				var attribute = (ObjectAttribute)coreAttribute;
				ObjectAttributeHandle(ctx, attribute);
			}
		}

		private static void ObjectAttributeHandle(IObjectServiceContext ctx, ObjectAttribute attribute) {
			var objectType = attribute.OwnerType;
			if(objectType == null) {
				return;
			}
			var lifetime = attribute.Lifetime;

			Type containerType = null;
			if (!string.IsNullOrEmpty(lifetime)) {
				containerType = ctx.ObjectService.GetOrCreateType(lifetime);
			}
			IObjectContainer container = null;
			if (containerType != null) {
				container = (IObjectContainer)TypeHelper.CreateObject(containerType, typeof(IObjectContainer), false);
			}
			if (container == null) {
				container = ObjectContainer.CreateDefault();
			}

			IObjectBuilder objectBuilder = null;
			var builderName = attribute.Builder;
			if(!string.IsNullOrEmpty(builderName)) {
				objectBuilder = ctx.ObjectService.GetObject<IObjectBuilder>(builderName);
			}
			if(objectBuilder == null) {
				objectBuilder = ctx.ObjectService.ObjectBuilder;
			}
			var description = ObjectDescription.CreateFromType(objectType);
			container.Init(description, objectBuilder);

			var name = attribute.Name;
			if (string.IsNullOrEmpty(name)) {
				name = objectType.FullName;
			}
			var objects = ctx.GlobalObjectNamespace;
			objects.AddObject(name, container);
			objects.AddObject(objectType, container);

			//查找此类型实现的接口
			var interfaces = objectType.GetInterfaces();
			if (interfaces.Length > 0) {
				foreach (var ifType in interfaces) {
					if (ifType.IsDefined(typeof(ServiceAttribute), true)) {
						objects.AddObject(ifType, container);
					}
				}
			}
		}

		//初始化类型别名
		private static void InitTypeAliases(IObjectNamespace space, IObjectNamespaceSetting setting) {
			if (space == null || setting == null || setting.TypeAliases == null || setting.TypeAliases.Length <= 0) {
				return;
			}

			var aliases = space.TypeAliases;
			foreach (var typeSetting in setting.TypeAliases) {
				var typeName = typeSetting.TypeName;
				if(string.IsNullOrEmpty(typeName)) {
					continue;
				}
				var type = TypeHelper.CreateType(typeName, false);
				if(type == null) {
					continue;
				}
				var name = typeSetting.Name;
				aliases.Add(name, type);
			}
		}

		//初始化常量
		private void InitConstAliases(IObjectNamespace space, IObjectNamespaceSetting setting) {
			if (space == null || setting == null || setting.ConstAliases == null || setting.ConstAliases.Length <= 0) {
				return;
			}

			var aliases = space.ConstAliases;
			foreach (var constSetting in setting.ConstAliases) {
				var type = this.GetOrCreateType(constSetting.TypeName);
				if(type == null) {
					continue;
				}
				var value = Convert.ChangeType(constSetting.Value, type);
				var name = constSetting.Name;
				aliases.Add(name, value);
			}
		}

		//初始化对象
		private void InitObjectAliases(ObjectNamespace space, IObjectNamespaceSetting setting, Func<Type, IObjectContainer> predicate) {
			if (space == null || setting == null || setting.Objects == null || setting.Objects.Length <= 0) {
				return;
			}

			foreach (var objectSetting in setting.Objects) {
				var objectType = this.GetOrCreateType(objectSetting.TypeName);
				if(objectType == null) {
					continue;
				}
				var objectName = objectSetting.Name;
				var container = predicate(objectType);
				if (container == null) {
					var lifetimeTypeName = objectSetting.Lifetime;
					if (!string.IsNullOrEmpty(lifetimeTypeName)) {
						var lifetimeType = this.GetOrCreateType(lifetimeTypeName);
						container = (IObjectContainer)TypeHelper.CreateObject(lifetimeType, typeof(IObjectContainer), false);
					}
				}
				if(container == null) {
					container = ObjectContainer.CreateDefault();
				}
				space.AddObject(objectName, container);//GetObject(aliasName)时可返回
				space.AddObject(objectType, container);//GetObject<>时可返回

				IObjectBuilder objectBuilder = null;
				var builderName = objectSetting.Builder;
				if (!string.IsNullOrEmpty(builderName)) {
					objectBuilder = ((IObjectService)this).GetObject<IObjectBuilder>(builderName);
				}
				if (objectBuilder == null) {
					objectBuilder = this.ObjectBuilder;
				}
				var description = ObjectDescription.CreateFromSetting(this, objectType, objectSetting);
				container.Init(description, objectBuilder);

				//查找此类型实现的接口
				var interfaces = objectType.GetInterfaces();
				if (interfaces.Length > 0) {
					foreach (var ifType in interfaces) {
						if (ifType.IsDefined(typeof(ServiceAttribute), true)) {
							space.AddObject(ifType, container);
						}
					}
				}
			}
		}

		//处理CoreAttribute
		private static void CoreAttributeHandle(IObjectServiceContext ctx) {
			var attributes = ctx.CoreAttributes;
			if (!attributes.TryGetValue(typeof(CoreAttributeHandlerAttribute), out var list)) {
				return;
			}
			var items = ctx.Items;
			var orderList = list.OrderByDescending(x => x.Priority);
			var intfName = typeof(ICoreAttributeHandler<>).FullName;
			foreach (var coreAttribute in orderList) {
				var attribute = (CoreAttributeHandlerAttribute)coreAttribute;
				var type = attribute?.OwnerType;
				if(type == null) {
					continue;
				}
				var interfaces = type.GetInterfaces();
				if(interfaces == null || interfaces.Length <= 0) {
					continue;
				}
				var instance = TypeHelper.CreateObject(type, null, false);
				if(instance == null) {
					continue;
				}
				foreach(var intf in interfaces) {
					if(!intf.IsGenericType) {
						continue;
					}
					if(intf.GetGenericTypeDefinition() != typeof(ICoreAttributeHandler<>)) {
						continue;
					}
					var args = intf.GetGenericArguments();
					var attributeType = args[0];
					if(!typeof(CoreAttribute).IsAssignableFrom(attributeType)) {
						continue;
					}
					var method = intf.GetMethod("HandleAttributes");
					if(method != null) {
						if (!attributes.TryGetValue(attributeType, out var list1)) {
							continue;
						}
						method.Invoke(instance, new object[] { ctx, list1 });
					}
				}
			}
		}

		internal virtual void InitCompletedInternal(IDictionary items) {
			foreach (var container in this.globalObjectNamespace.ObjectAliases.Values) {
				container.InitCompleted();
			}
			var ctx = new ObjectServiceContext(this, items) {
				GlobalObjectNamespace = this.globalObjectNamespace,
				CoreAttributes = this.CoreAttributes
			};
			this.OnInitCompleted(ctx);
		}

		internal virtual void SetupCompletedInternal(IDictionary items) {
			var ctx = new ObjectServiceContext(this, items) {
				GlobalObjectNamespace = this.globalObjectNamespace,
				CoreAttributes = this.CoreAttributes
			};
			this.OnSetupCompleted(ctx);
		}

		#endregion

		#region GetObjectContainer

		protected virtual IObjectContainer GetObjectContainer(Type objectKey, IDictionary items) {
			lock(objectKey) {
				var ctx = new ObjectContainerContext(objectKey, items: items) { Namespace = this.globalObjectNamespace, ObjectService = this };
				this.OnObjectContainerGetting(ctx);
				var container = ctx.Container;

				if (container == null) {
					if(this.globalObjectNamespace.TypedObjects.ContainsKey(objectKey)) {
						var typedObject = this.globalObjectNamespace.TypedObjects[objectKey];
						if(typedObject != null) {
							container = typedObject.Container;
						}
					}
				}

				ctx.Container = container;
				this.OnObjectContainerGetted(ctx);
				container = ctx.Container;

				return container;
			}
		}

		protected virtual IObjectContainer GetObjectContainer(string objectKey, IDictionary items) {
			lock(objectKey) {
				var ctx = new ObjectContainerContext(objectKey, items: items) { Namespace = this.globalObjectNamespace, ObjectService = this };
				this.OnObjectContainerGetting(ctx);
				var container = ctx.Container;

				if (container == null) {
					this.globalObjectNamespace.ObjectAliases.TryGetValue(objectKey, out container);
				}

				ctx.Container = container;
				this.OnObjectContainerGetted(ctx);
				container = ctx.Container;

				return container;
			}
		}

		#endregion

		#region GetObjectInternal

		protected virtual T GetObjectInternal<T>(IDictionary items) {
			return (T)this.GetObjectInternal(typeof(T), default(T), items);
		}

		protected virtual object GetObjectInternal(Type type, object defaultValue, IDictionary items) {
			if(typeof(IObjectService).IsAssignableFrom(type) || typeof(IObjectServiceInternal).IsAssignableFrom(type)) {
				return this;
			}

			var container = this.GetObjectContainer(type, items);
			return container != null ? container.GetObject(items) : defaultValue;
		}

		protected virtual object GetObjectInternal(string objectKey, IDictionary items) {
			object value = null;
			var container = this.GetObjectContainer(objectKey, items);
			if(container != null) {
				value = container.GetObject(items);
			}
			return value;
		}

		protected virtual object[] GetObjectsInternal(Type objectType, IDictionary items) {
			var values = new List<object>();
			if (this.globalObjectNamespace.TypedObjects.TryGetValue(objectType, out var typedObject)) {
				foreach (var container in typedObject.Containers) {
					values.Add(container.GetObject(items));
				}
			}
			if (values.Count > 0) {
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

		#region GetTypeInternal

		protected internal virtual Type GetTypeInternal(string typeAlias) {
			this.globalObjectNamespace.TypeAliases.TryGetValue(typeAlias, out var type);
			return type;
		}

		#endregion

		#region CreateObject

		protected virtual T CreateObjectInternal<T>(IDictionary items, params object[] args) {
			return (T)this.CreateObjectInternal(typeof(T), default(T), items, args);
		}

		protected virtual object CreateObjectInternal(Type type, object defaultValue, IDictionary items, params object[] args) {
			var container = this.GetObjectContainer(type, items);
			object instance;
			if (container != null) {
				instance = this.CreateObjectInternal(container, items, args);
			} else {
				instance = this.ObjectBuilder.CreateObject(new ObjectDescription(type, items), items, args);
			}
			return instance ?? defaultValue;
		}

		protected virtual object CreateObjectInternal(string objectAlias, object defaultValue, IDictionary items, params object[] args) {
			var container = this.GetObjectContainer(objectAlias, items);
			object instance = null;
			if (container != null) {
				instance = this.CreateObjectInternal(container, items, args);
			} else {
				var type = this.GetOrCreateType(objectAlias);
				if(type != null) {
					instance = this.ObjectBuilder.CreateObject(new ObjectDescription(type, items), items, args);
				}
			}
			return instance ?? defaultValue;
		}

		protected virtual object CreateObjectInternal(IObjectContainer objectContainer, IDictionary items, params object[] args) {
			return objectContainer.ObjectBuilder.CreateObject(objectContainer.ObjectDescription, items, args);
		}

		#endregion

		#region Properties and fields

		protected internal const string GlobalObjectNamespaceName = "";
		protected virtual IObjectBuilder ObjectBuilder { get; private set; }
		protected virtual IConfigSetting ConfigSetting { get; private set; }
		protected virtual IDictionary<Type, IList<CoreAttribute>> CoreAttributes { get; private set; }
		private ObjectNamespace globalObjectNamespace;

		#endregion Properties and fields
	}
}
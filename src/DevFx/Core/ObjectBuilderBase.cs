using System;
using System.Collections;
using DevFx.Reflection;
using DevFx.Utils;

namespace DevFx.Core
{
	public abstract class ObjectBuilderBase : IObjectBuilder
	{
		protected virtual IObjectService ObjectService { get; }
		public ObjectBuilderBase(IObjectService objectService) {
			this.ObjectService = objectService;
		}

		protected virtual void OnObjectCreating(IObjectBuilderContext ctx) {
			var objectService = (IObjectServiceInternal)ctx.ObjectService;
			objectService?.OnObjectCreating(ctx);
		}

		protected virtual void OnObjectCreated(IObjectBuilderContext ctx) {
			var objectService = (IObjectServiceInternal)ctx.ObjectService;
			objectService?.OnObjectCreated(ctx);
		}

		protected virtual object CreateObject(IObjectDescription objectDescription, IDictionary items, params object[] args) {
			var ctx = new ObjectBuilderContext(this.ObjectService, this, objectDescription, items);
			this.OnObjectCreating(ctx);
			var instance = ctx.ObjectInstance;
			if(instance != null) {
				return instance;
			}
			this.CreateObjectInternal(ctx, args);

			this.OnObjectCreated(ctx);
			instance = ctx.ObjectInstance;
			return instance;
		}

		protected virtual void CreateObjectInternal(IObjectBuilderContext ctx, params object[] args) {
			var instance = BuildObject(ctx.ObjectService, ctx.ObjectDescription, ctx.Items, args);
			ctx.ObjectInstance = instance;
		}

		public static object BuildObject(IObjectService objectService, IObjectDescription description, IDictionary items, params object[] args) {
			if(description == null) {
				return null;
			}
			var objectType = description.ObjectType;
			if(objectType.IsInterface) {
				throw new Exception("无法创建接口的实例");
			}
			object instance = null;
			if(args != null && args.Length > 0) {
				instance = TypeHelper.CreateObject(objectType, null, true, args);
			}
			if(instance == null) {
				var parameters = description.ConstructorParameters;
				if(parameters != null) {
					var length = parameters.Length;
					if(length > 0) {
						var types = new Type[length];
						var values = new object[length];
						for(var i = 0; i < parameters.Length; i++) {
							var parameter = parameters[i];
							types[i] = parameter.Type;
							values[i] = GetValueFromValueDescription(parameter, objectService);
						}
						instance = TypeHelper.CreateObject(objectType, null, true, types, values);
					}
				}
			}
			if(instance == null) {
				instance = TypeHelper.CreateObject(objectType, null, true);
			}
			//尝试进行属性注入
			var properties = description.Properties;
			if(instance != null && properties != null && properties.Length > 0) {
				foreach(var property in properties) {
					var name = property.Name;
					var value = GetValueFromValueDescription(property, objectService);
					if(value == null && property.ValueRequired) {
						throw new Exception($"value of {description.ObjectType}.{property.Name} cannot be resolved.");
					}
					if(instance is IPropertyDescriptor descriptor) {
						descriptor.SetValue(name, value);
					} else {
						TypeHelper.SetPropertyValue(instance, name, value);
					}
				}
			}
			return instance;
		}

		public static object GetValueFromValueDescription(IValueDescription description, IObjectService objectService) {
			if(description == null) {
				return null;
			}
			var value = description.Value;
			var type = description.Type;
			if ((type.IsClass || type.IsInterface) && type != typeof(string)) {
				//尝试进行注入
				if (value is string value1 && !string.IsNullOrEmpty(value1)) {//类型引用
					value = objectService.GetObject(value1);
				} else if (value == null) {//强类型
					value = objectService.GetObject(type);
				}
			} else {//type 是基础类型或者是string类型
				if(value is string value1) {
					value = Converting.GetConvert(value1).TryToObject(type);
				} else {
					value = Convert.ChangeType(value, type);
				}
			}
			return value;
		}

		#region IObjectBuilder Members

		object IObjectBuilder.CreateObject(IObjectDescription objectDescription, IDictionary items, params object[] args) {
			return this.CreateObject(objectDescription, items, args);
		}

		#endregion
	}
}

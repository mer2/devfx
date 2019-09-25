using DevFx.Core.Settings;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace DevFx.Core
{
	public interface IObjectDescription : IObjectContext
	{
		Type ObjectType { get; }
		IValueDescription[] ConstructorParameters { get; }
		IValueDescription[] Properties { get; }
	}

	public interface IValueDescription
	{
		string Name { get; }
		Type Type { get; }
		object Value { get; }
		bool ValueRequired { get; }
	}

	[Serializable]
	public class ValueDescription : IValueDescription
	{
		public string Name { get; set; }
		public Type Type { get; set; }
		public object Value { get; set; }
		public bool ValueRequired { get; set; }
	}

	public class ObjectDescription : ObjectContextBase, IObjectDescription
	{
		public ObjectDescription() : base(null) { }
		public ObjectDescription(Type objectType, IDictionary items = null) : base(items) {
			this.ObjectType = objectType;
		}

		public Type ObjectType { get; set; }
		public IValueDescription[] ConstructorParameters { get; set; }
		public IValueDescription[] Properties { get; set; }

		internal static ObjectDescription CreateFromType(Type objectType) {
			var description = new ObjectDescription(objectType);
			//获取需要被注入的构造方法
			var constructors = objectType.GetConstructors(BindingFlags.CreateInstance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
			foreach(var constructor in constructors) {
				if(constructor.IsDefined(typeof(AutowiredAttribute), true)) {
					var parameters = constructor.GetParameters();
					if(parameters.Length > 0) {
						description.ConstructorParameters = parameters.Select(x => (IValueDescription)new ValueDescription {
							Name = x.Name,
							Type = x.ParameterType
						}).ToArray();
					}
					break;
				}
			}
			//获取需要被注入的属性
			var properties = objectType.GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.SetProperty | BindingFlags.Instance);
			if(properties.Length > 0) {
				var list = new List<IValueDescription>();
				foreach(var property in properties) {
					if (property.IsDefined(typeof(AutowiredAttribute), true)) {
						var autowired = (AutowiredAttribute)property.GetCustomAttributes(typeof(AutowiredAttribute), true)[0];
						list.Add(new ValueDescription {
							Name = property.Name,
							Type = property.PropertyType,
							ValueRequired = autowired.Required
						});
					}
				}
				if (list.Count > 0) {
					description.Properties = list.ToArray();
				}
			}

			return description;
		}

		internal static ObjectDescription CreateFromSetting(IObjectService objectService, Type objectType, IObjectSetting setting) {
			var description = new ObjectDescription(objectType);
			var parameters = setting.ConstructorParameters;
			if(parameters != null && parameters.Length > 0) {
				description.ConstructorParameters = parameters.Select(x => {
					var value = new ValueDescription {
						Name = x.Name,
						Type = objectService.GetOrCreateType(x.TypeName),
						Value = x.Value
					};
					return (IValueDescription)value;
				}).ToArray();
			}
			var properties = setting.Properties;
			if(properties != null && properties.Length > 0) {
				description.Properties = properties.Select(x => {
					var value = new ValueDescription {
						Name = x.Name,
						Type = objectService.GetOrCreateType(x.TypeName),
						Value = x.Value
					};
					return (IValueDescription)value;
				}).ToArray();
			}
			return description;
		}
	}
}
/* Copyright(c) 2005-2011 R2@DevFx.NET, License(LGPL) */

using DevFx.Core;
using DevFx.Reflection;
using DevFx.Utils;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace DevFx.Configuration
{
	/// <summary>
	/// 强类型配置基类
	/// </summary>
	public abstract class ConfigSettingElement : IConfigSettingElement
	{
		private IConfigSetting configSetting;
		private int configVersion = -1;

		/// <summary>
		/// 获取或设置此强类型对应的配置节
		/// </summary>
		public virtual IConfigSetting ConfigSetting {
			get => this.configSetting;
			protected set {
				if(this.configSetting == value) {
					if(value != null && this.configVersion == value.Version) {
						return;
					}
				}
				this.configSetting = value;
				if (value != null) {
					this.configVersion = value.Version;
				}
				this.OnConfigSettingChanged();
			}
		}

		/// <summary>
		/// 配置节变化时将调用的方法
		/// </summary>
		protected virtual void OnConfigSettingChanged() {
		}

		#region IConfigSettingElement 成员

		IConfigSetting IConfigSettingElement.ConfigSetting {
			get => this.ConfigSetting;
			set => this.ConfigSetting = value;
		}

		#endregion

		public static IConfigSettingElement CreateProxy(IConfigSetting setting, Type settingType) {
			var instance = (ConfigSettingElementProxy)TypeHelper.CreateDispatchProxy(settingType, typeof(ConfigSettingElementProxy));
			instance.Wrapper = new ConfigSettingElementWrapper(setting, settingType);
			return instance;
		}
	}

	internal class ConfigSettingElementWrapper : ConfigSettingElement, IServiceDispatchProxy
	{
		public ConfigSettingElementWrapper(IConfigSetting setting, Type settingType) {
			if(settingType == null) {
				return;
			}
			var properties = settingType.GetProperties();
			foreach(var property in properties) {
				var settingName = property.GetCustomAttribute<SettingNameAttribute>();
				var name = settingName != null ? settingName.Name : property.Name;
				var value = new ValueDescription {
					Name = property.Name,
					Type = property.PropertyType
				};
				if(value.Type.IsValueType) {
					value.Value = Convert.ChangeType(0, value.Type);
				}
				this.values.Add(name, value);
			}
			this.ConfigSetting = setting;
		}

		/// <summary>
		/// 配置节变化时将调用的方法
		/// </summary>
		protected override void OnConfigSettingChanged() {
			foreach(var description in this.values.Values) {
				var name = description.Name;
				var type = description.Type;
				var value = description.Value;
				if(type == typeof(string)) {//字符串，则直接赋值
					description.Value = this.ConfigSetting.GetSetting<string>(name);
				} else if (type.IsValueType) {//如果是值类型，则直接赋值
					description.Value = this.ConfigSetting.GetSetting(name, type, Convert.ChangeType(0, type));
				} else if(value is IConfigSettingElement settingElement) {
					settingElement.ConfigSetting = this.ConfigSetting[name];
				} else if(type.IsInterface) {//动态创建子属性
					var child = CreateProxy(this.ConfigSetting[name], type);
					description.Value = child;
				} else if(type.IsArray) {//数组
					var elementType = type.GetElementType();
					if(elementType.IsInterface) {
						var children = this.ConfigSetting[name]?.GetChildSettings();
						if(children != null && children.Length > 0) {
							var values = Array.CreateInstance(elementType, children.Length);
							for(var i = 0; i < children.Length; i++) {
								var child = children[i];
								var element = CreateProxy(child, elementType);
								values.SetValue(element, i);
							}
							description.Value = values;
						}
					}
				}
			}
		}

		private readonly IDictionary<string, ValueDescription> values = new Dictionary<string, ValueDescription>();
		object IServiceDispatchProxy.Invoke(MethodInfo targetMethod, object[] args) {
			var name = targetMethod.Name;
			var declareType = targetMethod.DeclaringType;
			if (declareType == typeof(IConfigSettingElement)) {
				if(targetMethod.IsSpecialName && name.StartsWith("get_")) {
					if (name.StartsWith("get_")) {
						return this.ConfigSetting;
					}
					if(name.StartsWith("set_")) {
						this.ConfigSetting = (IConfigSetting)args[0];
						return null;
					}
				}
			}
			if(targetMethod.IsSpecialName && name.StartsWith("get_")) {
				name = name.Substring(4);
				this.values.TryGetValue(name, out var value);
				return value?.Value;
			}
			throw new System.NotImplementedException(name);
		}
	}

	public class ConfigSettingElementProxy : ServiceDispatchProxy, IConfigSettingElement
	{
		IConfigSetting IConfigSettingElement.ConfigSetting {
			get => ((IConfigSettingElement)this.Wrapper).ConfigSetting;
			set => ((IConfigSettingElement)this.Wrapper).ConfigSetting = value;
		}
	}
}

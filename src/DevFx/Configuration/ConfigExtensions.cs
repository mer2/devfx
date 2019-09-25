using DevFx.Utils;
using System;
using System.Collections.Generic;

namespace DevFx.Configuration
{
	public static class ConfigExtensions
	{
		/// <summary>
		/// 复制配置节到当前节点，与<see cref="IConfigSetting.Merge"/>方法类似，但复制方向正好相反，且配置节名可以不一致
		/// </summary>
		/// <param name="configSetting">需要复制的配置节</param>
		/// <param name="fromSetting">被复制的配置节</param>
		/// <returns>合并后的配置节</returns>
		public static IConfigSetting CopyFrom(this IConfigSetting configSetting, IConfigSetting fromSetting) {
			if (configSetting is ConfigSetting setting) {
				setting.CopyFrom(fromSetting as ConfigSetting);
			}
			return configSetting;
		}

		/// <summary>
		/// 获取配置值
		/// </summary>
		/// <param name="configSetting">配置节</param>
		/// <param name="name">名称</param>
		/// <returns>配置值</returns>
		public static ISettingValue GetSettingValue(this IConfigSetting configSetting, string name) {
			if (configSetting == null) {
				return null;
			}
			var settingValue = configSetting.Property[name];
			if (settingValue == null) {
				var setting = configSetting[name];
				if (setting != null) {
					settingValue = setting.Value;
				}
			}
			return settingValue;
		}

		/// <summary>
		/// 获取配置值
		/// </summary>
		/// <typeparam name="T">值类型</typeparam>
		/// <param name="configSetting">配置节</param>
		/// <param name="name">值名</param>
		/// <param name="defaultValue">值缺省值</param>
		/// <param name="required">是否必须配置</param>
		/// <returns>配置值</returns>
		public static object GetSetting(this IConfigSetting configSetting, string name, Type type, object defaultValue = null, bool required = false) {
			if(configSetting == null) {
				return defaultValue;
			}
			if(type == typeof(string) || type.IsValueType) {
				var settingValue = configSetting.GetSettingValue(name);
				if(settingValue == null && required) {
					throw new ConfigException($"{configSetting.Name} 必须配置 {name} 节点");
				}
				return settingValue == null ? defaultValue : settingValue.TryToObject(type, defaultValue);
			}
			if (typeof(IConfigSettingElement).IsAssignableFrom(type)) {
				var setting = name == null ? configSetting : configSetting[name];
				if(setting == null && required) {
					throw new ConfigException($"{configSetting.Name} 必须配置 {name} 节点");
				}
				return setting.ToSetting(type);
			}
			return defaultValue;
		}

		/// <summary>
		/// 把配置节转化成强类型配置类
		/// </summary>
		/// <param name="configSetting">配置节</param>
		/// <param name="objectType">强类型配置类型</param>
		/// <param name="xpath">需被转换的配置节路径</param>
		/// <param name="required">是否必须配置</param>
		/// <returns>强类型配置实例</returns>
		public static object ToSetting(this IConfigSetting configSetting, Type objectType, string xpath = null, bool required = false) {
			if (configSetting == null) {
				return null;
			}
			var setting = configSetting;
			if (!string.IsNullOrEmpty(xpath)) {
				setting = configSetting.GetChildSetting(xpath);
			}
			if (setting == null) {
				if(required) {
					throw new ConfigException($"{configSetting.Name} 必须配置 {xpath} 节点");
				}
				return null;
			}
			var elementType = typeof(IConfigSettingElement);
			if (elementType.IsAssignableFrom(objectType)) {
				if (TypeHelper.CreateObject(objectType, elementType, true) is IConfigSettingElement instance) {
					instance.ConfigSetting = setting;
					return instance;
				}
			}
			return null;
		}

		/// <summary>
		/// 获取配置值
		/// </summary>
		/// <typeparam name="T">值类型</typeparam>
		/// <param name="configSetting">配置节</param>
		/// <param name="name">值名</param>
		/// <param name="defaultValue">值缺省值</param>
		/// <param name="required">是否必须配置</param>
		/// <returns>配置值</returns>
		public static T GetSetting<T>(this IConfigSetting configSetting, string name, T defaultValue = default, bool required = false) {
			return (T)configSetting.GetSetting(name, typeof(T), defaultValue, required);
		}

		/// <summary>
		/// 把配置节转化成强类型配置类
		/// </summary>
		/// <typeparam name="T">强类型配置类型</typeparam>
		/// <param name="configSetting">配置节</param>
		/// <returns>强类型配置实例</returns>
		public static T ToSetting<T>(this IConfigSetting configSetting) where T : IConfigSettingElement, new() {
			return configSetting == null ? default : new T { ConfigSetting = configSetting };
		}

		/// <summary>
		/// 把配置节转化成强类型配置类
		/// </summary>
		/// <typeparam name="T">强类型配置类型</typeparam>
		/// <param name="configSetting">配置节</param>
		/// <param name="xpath">需被转换的配置节路径</param>
		/// <param name="required">是否必须配置</param>
		/// <returns>强类型配置实例</returns>
		public static T ToSetting<T>(this IConfigSetting configSetting, string xpath = null, bool required = false) {
			return (T)configSetting.ToSetting(typeof(T), xpath, required);
		}

		/// <summary>
		/// 获取配置值
		/// </summary>
		/// <typeparam name="T">值类型</typeparam>
		/// <param name="settingElement">强类型配置实例</param>
		/// <param name="name">值名</param>
		/// <param name="defaultValue">值缺省值</param>
		/// <param name="required">是否必须配置</param>
		/// <returns>配置值</returns>
		public static T GetSetting<T>(this IConfigSettingElement settingElement, string name, T defaultValue = default, bool required = false) {
			return settingElement.ConfigSetting.GetSetting(name, defaultValue, required);
		}

		/// <summary>
		/// 获取配置值集合
		/// </summary>
		/// <typeparam name="T">值类型</typeparam>
		/// <param name="name">子节名</param>
		/// <returns>配置值集合</returns>
		public static List<T> GetSettings<T>(this IConfigSettingElement element, string name) where T : IConfigSettingElement, new() {
			return GetSettingsWithInitializer<T>(element, name, null);
		}

		/// <summary>
		/// 获取配置值集合
		/// </summary>
		/// <typeparam name="T">值类型</typeparam>
		/// <param name="name">子节名</param>
		/// <param name="initializer">对元素进行初始化</param>
		/// <returns>配置值集合</returns>
		public static List<T> GetSettingsWithInitializer<T>(this IConfigSettingElement element, string name, Action<T> initializer) where T : IConfigSettingElement, new() {
			var list = new List<T>();
			var setting = element.ConfigSetting;
			if (setting != null) {
				var settings = string.IsNullOrEmpty(name) ? setting.GetChildSettings() : setting.GetChildSettings(name);
				foreach (var item in settings) {
					var t = item.ToSetting<T>();
					initializer?.Invoke(t);
					list.Add(t);
				}
			}
			return list;
		}

		/// <summary>
		/// 获取配置值集合
		/// </summary>
		/// <typeparam name="T">值类型</typeparam>
		/// <param name="name">子节名</param>
		/// <param name="settingName">子节名称</param>
		/// <returns>配置值集合</returns>
		public static List<T> GetSettings<T>(this IConfigSettingElement element, string name, string settingName) where T : IConfigSettingElement, new() {
			return GetSettingsWithInitializer<T>(element, name, settingName, null);
		}

		/// <summary>
		/// 获取配置值集合
		/// </summary>
		/// <typeparam name="T">值类型</typeparam>
		/// <param name="name">子节名</param>
		/// <param name="settingName">子节名称</param>
		/// <param name="initializer">对元素进行初始化</param>
		/// <returns>配置值集合</returns>
		public static List<T> GetSettingsWithInitializer<T>(this IConfigSettingElement element, string name, string settingName, Action<T> initializer) where T : IConfigSettingElement, new() {
			var list = new List<T>();
			var setting = element.ConfigSetting;
			if (setting != null) {
				setting = setting[name];
			}
			if (setting != null) {
				var settings = string.IsNullOrEmpty(settingName) ? setting.GetChildSettings() : setting.GetChildSettings(settingName);
				foreach (var item in settings) {
					var t = item.ToSetting<T>();
					initializer?.Invoke(t);
					list.Add(t);
				}
			}
			return list;
		}
	}
}

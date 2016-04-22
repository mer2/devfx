/* Copyright(c) 2005-2011 R2@DevFx.NET, License(LGPL) */

using System;
using System.Collections.Generic;
using System.Linq;
using HTB.DevFx.Utils;

namespace HTB.DevFx.Config
{
	/// <summary>
	/// 有关配置的扩展方法集
	/// </summary>
	public static class ConfigExtensions
	{
		#region IConfigSetting Merge Extensions

		/// <summary>
		/// 复制配置节到当前节点，与<see cref="IConfigSetting.Merge"/>方法类似，但复制方向正好相反，且配置节名可以不一致
		/// </summary>
		/// <param name="configSetting">需要复制的配置节</param>
		/// <param name="fromSetting">被复制的配置节</param>
		/// <returns>合并后的配置节</returns>
		public static IConfigSetting CopyFrom(this IConfigSetting configSetting, IConfigSetting fromSetting) {
			var setting = configSetting as ConfigSetting;
			if(setting != null) {
				setting.CopyFrom(fromSetting as ConfigSetting);
			}
			return configSetting;
		}

		#endregion

		#region IConfigSetting Extensions

		/// <summary>
		/// 把配置节转化成强类型配置类
		/// </summary>
		/// <typeparam name="T">强类型配置类型</typeparam>
		/// <param name="configSetting">配置节</param>
		/// <returns>强类型配置实例</returns>
		public static T ToSetting<T>(this IConfigSetting configSetting) where T : IConfigSettingElement, new() {
			return configSetting == null ? default(T) : new T { ConfigSetting = configSetting };
		}

		/// <summary>
		/// 把配置节转化成强类型配置类
		/// </summary>
		/// <typeparam name="T">强类型配置类型</typeparam>
		/// <param name="configSetting">配置节</param>
		/// <returns>强类型配置实例</returns>
		public static T ToCachedSetting<T>(this IConfigSetting configSetting) where T : IConfigSettingElement, new() {
			return configSetting != null ? configSetting.GetObjectContext<T>(typeof(T), () => new T { ConfigSetting = configSetting }) : default(T);
		}

		/// <summary>
		/// 把配置节转化成强类型配置类
		/// </summary>
		/// <typeparam name="T">强类型配置类型</typeparam>
		/// <param name="configSetting">配置节</param>
		/// <param name="xpath">需被转换的配置节路径</param>
		/// <returns>强类型配置实例</returns>
		public static T ToSetting<T>(this IConfigSetting configSetting, string xpath) where T : IConfigSettingElement, new() {
			if(configSetting != null) {
				var setting = configSetting.GetChildSetting(xpath);
				if(setting != null) {
					return new T { ConfigSetting = setting };
				}
			}
			return default(T);
		}

		/// <summary>
		/// 把配置节转化成强类型配置类
		/// </summary>
		/// <typeparam name="T">强类型配置类型</typeparam>
		/// <param name="configSetting">配置节</param>
		/// <param name="xpath">需被转换的配置节路径</param>
		/// <returns>强类型配置实例</returns>
		public static T ToCachedSetting<T>(this IConfigSetting configSetting, string xpath) where T : IConfigSettingElement, new() {
			return configSetting != null ? configSetting.GetChildSetting(xpath).ToCachedSetting<T>() : default(T);
		}

		/// <summary>
		/// 获取配置值
		/// </summary>
		/// <typeparam name="T">值类型</typeparam>
		/// <param name="configSetting">配置节</param>
		/// <param name="name">值名</param>
		/// <param name="defaultValue">值缺省值</param>
		/// <returns>配置值</returns>
		public static T GetSetting<T>(this IConfigSetting configSetting, string name, T defaultValue) {
			ISettingValue settingValue = null;
			if (configSetting != null) {
				settingValue = configSetting.Property[name];
				if(settingValue == null) {
					var setting = name == null ? configSetting : configSetting[name];
					if (setting != null) {
						var type = typeof(T);
						if (typeof(IConfigSettingElement).IsAssignableFrom(typeof(T))) {
							return (T)setting.ToSetting(type);
						}
						settingValue = setting.Value;
					}
				}
			}
			return settingValue != null ? settingValue.TryToObject<T>() : defaultValue;
		}

		/// <summary>
		/// 获取配置值
		/// </summary>
		/// <typeparam name="T">值类型</typeparam>
		/// <param name="configSetting">配置节</param>
		/// <param name="name">值名</param>
		/// <returns>配置值</returns>
		public static T GetSetting<T>(this IConfigSetting configSetting, string name) {
			return configSetting.GetSetting(name, default(T));
		}

		/// <summary>
		/// 获取配置值
		/// </summary>
		/// <typeparam name="T">值类型</typeparam>
		/// <param name="configSetting">配置节</param>
		/// <returns>配置值</returns>
		public static T GetSetting<T>(this IConfigSetting configSetting) {
			return configSetting.GetSetting<T>(null);
		}

		/// <summary>
		/// 获取配置值
		/// </summary>
		/// <param name="configSetting">配置节</param>
		/// <param name="name">值名</param>
		/// <returns>配置值</returns>
		public static string GetSetting(this IConfigSetting configSetting, string name) {
			var settingValue = GetSettingValue(configSetting, name);
			return settingValue == null ? null : settingValue.Value;
		}

		/// <summary>
		/// 获取配置值集合
		/// </summary>
		/// <typeparam name="T">值类型</typeparam>
		/// <param name="configSetting">配置节</param>
		/// <param name="name">值名</param>
		/// <returns>配置值集合</returns>
		public static ICollection<T> GetSettings<T>(this IConfigSetting configSetting, string name) where T : IConfigSettingElement, new() {
			return configSetting.GetSettingsWithInitializer(name, (Action<T>)null);
		}

		/// <summary>
		/// 获取配置值集合
		/// </summary>
		/// <typeparam name="T">值类型</typeparam>
		/// <param name="configSetting">配置节</param>
		/// <param name="name">值名</param>
		/// <param name="initializer">对元素进行初始化</param>
		/// <returns>配置值集合</returns>
		public static ICollection<T> GetSettingsWithInitializer<T>(this IConfigSetting configSetting, string name, Action<T> initializer) where T : IConfigSettingElement, new() {
			var list = new List<T>();
			if (configSetting != null) {
				var settings = configSetting.GetChildSettings(name);
				if (settings != null && settings.Length > 0) {
					foreach (var item in settings) {
						var t = item.ToSetting<T>();
						if(initializer != null) {
							initializer(t);
						}
						list.Add(t);
					}
				}
			}
			return list;
		}

		/// <summary>
		/// 获取配置值集合
		/// </summary>
		/// <typeparam name="T">值类型</typeparam>
		/// <param name="configSetting">配置节</param>
		/// <param name="name">值名</param>
		/// <param name="settingName">集合值名</param>
		/// <returns>配置值集合</returns>
		public static ICollection<T> GetSettings<T>(this IConfigSetting configSetting, string name, string settingName) where T : IConfigSettingElement, new() {
			return configSetting.GetSettingsWithInitializer(name, settingName, (Action<T>)null);
		}

		/// <summary>
		/// 获取配置值集合
		/// </summary>
		/// <typeparam name="T">值类型</typeparam>
		/// <param name="configSetting">配置节</param>
		/// <param name="name">值名</param>
		/// <param name="settingName">集合值名</param>
		/// <param name="initializer">对元素进行初始化</param>
		/// <returns>配置值集合</returns>
		public static ICollection<T> GetSettingsWithInitializer<T>(this IConfigSetting configSetting, string name, string settingName, Action<T> initializer) where T : IConfigSettingElement, new() {
			var list = new List<T>();
			if (configSetting != null) {
				var setting = configSetting[name];
				if (setting != null) {
					var settings = string.IsNullOrEmpty(settingName) ? setting.GetChildSettings() : setting.GetChildSettings(settingName);
					foreach (var item in settings) {
						var t = item.ToSetting<T>();
						if(initializer != null) {
							initializer(t);
						}
						list.Add(t);
					}
				}
			}
			return list;
		}

		/// <summary>
		/// 转换为数组
		/// </summary>
		/// <typeparam name="T">数组元素</typeparam>
		/// <param name="collection">集合</param>
		/// <returns>数组</returns>
		public static T[] ToArray<T>(this ICollection<T> collection) {
			T[] array = null;
			if(collection != null) {
				if(collection is List<T>) {
					array = ((List<T>)collection).ToArray();
				} else if(collection is T[]) {
					array = (T[])collection;
				} else {
					array = Enumerable.ToArray(collection);
				}
			}
			return array;
		}

		private static ISettingValue GetSettingValue(IConfigSetting configSetting, string name) {
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

		private static object ToSetting(this IConfigSetting configSetting, Type objectType) {
			if(configSetting != null) {
				var elementType = typeof(IConfigSettingElement);
				if (elementType.IsAssignableFrom(objectType)) {
					var instance = TypeHelper.CreateObject(objectType, elementType, true) as IConfigSettingElement;
					if (instance != null) {
						instance.ConfigSetting = configSetting;
						return instance;
					}
				}
			}
			return null;
		}

		#endregion IConfigSetting Extensions

		#region IConfigSetting Required Extensions

		/// <summary>
		/// 获取必须配置的值
		/// </summary>
		/// <typeparam name="T">值类型</typeparam>
		/// <param name="configSetting">配置节</param>
		/// <param name="name">值名</param>
		/// <returns>配置值</returns>
		public static T GetRequiredSetting<T>(this IConfigSetting configSetting, string name) {
			ISettingValue settingValue = null;
			if (configSetting != null) {
				settingValue = configSetting.Property[name];
				if (settingValue == null) {
					var setting = name == null ? configSetting : configSetting[name];
					if (setting != null) {
						var type = typeof(T);
						if (typeof(IConfigSettingElement).IsAssignableFrom(typeof(T))) {
							return (T)setting.ToSetting(type);
						}
						settingValue = setting.Value;
					}
				}
			}
			if(settingValue != null) {
				return settingValue.TryToObject<T>();
			}
			throw new ConfigException(string.Format("必须配置节点 {0}", name));
		}

		/// <summary>
		/// 获取必须配置的值
		/// </summary>
		/// <typeparam name="T">值类型</typeparam>
		/// <param name="configSetting">配置节</param>
		/// <returns>配置值</returns>
		public static T GetRequiredSetting<T>(this IConfigSetting configSetting) {
			return configSetting.GetRequiredSetting<T>(null);
		}

		/// <summary>
		/// 获取必须配置的值
		/// </summary>
		/// <param name="configSetting">配置节</param>
		/// <param name="name">值名</param>
		/// <returns>配置值</returns>
		public static string GetRequiredSetting(this IConfigSetting configSetting, string name) {
			var settingValue = GetSettingValue(configSetting, name);
			if(settingValue != null) {
				return settingValue.Value;
			}
			throw new ConfigException(string.Format("必须配置节点 {0}", name));
		}

		/// <summary>
		/// 获取必须配置的值集合
		/// </summary>
		/// <typeparam name="T">值类型</typeparam>
		/// <param name="configSetting">配置节</param>
		/// <param name="name">值名</param>
		/// <returns>配置值集合</returns>
		public static ICollection<T> GetRequiredSettings<T>(this IConfigSetting configSetting, string name) where T : IConfigSettingElement, new() {
			return configSetting.GetSettingsWithInitializer(name, (Action<T>)null);
		}

		/// <summary>
		/// 获取配置值集合
		/// </summary>
		/// <typeparam name="T">值类型</typeparam>
		/// <param name="configSetting">配置节</param>
		/// <param name="name">值名</param>
		/// <param name="initializer">对元素进行初始化</param>
		/// <returns>配置值集合</returns>
		public static ICollection<T> GetRequiredSettingsWithInitializer<T>(this IConfigSetting configSetting, string name, Action<T> initializer) where T : IConfigSettingElement, new() {
			var list = new List<T>();
			if (configSetting != null) {
				var settings = configSetting.GetChildSettings(name);
				if (settings != null && settings.Length > 0) {
					foreach (var item in settings) {
						var t = item.ToSetting<T>();
						if (initializer != null) {
							initializer(t);
						}
						list.Add(t);
					}
				} else {
					throw new ConfigException(string.Format("必须配置节点 {0}", name));
				}
			}
			return list;
		}

		/// <summary>
		/// 获取配置值集合
		/// </summary>
		/// <typeparam name="T">值类型</typeparam>
		/// <param name="configSetting">配置节</param>
		/// <param name="name">值名</param>
		/// <param name="settingName">集合值名</param>
		/// <returns>配置值集合</returns>
		public static ICollection<T> GetRequiredSettings<T>(this IConfigSetting configSetting, string name, string settingName) where T : IConfigSettingElement, new() {
			return configSetting.GetRequiredSettingsWithInitializer(name, settingName, (Action<T>)null);
		}

		/// <summary>
		/// 获取配置值集合
		/// </summary>
		/// <typeparam name="T">值类型</typeparam>
		/// <param name="configSetting">配置节</param>
		/// <param name="name">值名</param>
		/// <param name="settingName">集合值名</param>
		/// <param name="initializer">对元素进行初始化</param>
		/// <returns>配置值集合</returns>
		public static ICollection<T> GetRequiredSettingsWithInitializer<T>(this IConfigSetting configSetting, string name, string settingName, Action<T> initializer) where T : IConfigSettingElement, new() {
			var list = new List<T>();
			if (configSetting != null) {
				var setting = configSetting[name];
				if (setting != null) {
					var settings = string.IsNullOrEmpty(settingName) ? setting.GetChildSettings() : setting.GetChildSettings(settingName);
					foreach (var item in settings) {
						var t = item.ToSetting<T>();
						if (initializer != null) {
							initializer(t);
						}
						list.Add(t);
					}
				} else {
					throw new ConfigException(string.Format("必须配置节点 {0}", name));
				}
			}
			return list;
		}

		#endregion IConfigSetting Extensions

		#region IConfigSettingElement Extensions

		/// <summary>
		/// 获取配置值
		/// </summary>
		/// <typeparam name="T">值类型</typeparam>
		/// <param name="settingElement">强类型配置实例</param>
		/// <param name="name">值名</param>
		/// <param name="defaultValue">值缺省值</param>
		/// <returns>配置值</returns>
		public static T GetSetting<T>(this IConfigSettingElement settingElement, string name, T defaultValue) {
			return settingElement.ConfigSetting.GetSetting(name, defaultValue);
		}

		/// <summary>
		/// 获取配置值
		/// </summary>
		/// <typeparam name="T">值类型</typeparam>
		/// <param name="settingElement">强类型配置实例</param>
		/// <param name="name">值名</param>
		/// <returns>配置值</returns>
		public static T GetSetting<T>(this IConfigSettingElement settingElement, string name) {
			return settingElement.GetSetting(name, default(T));
		}

		/// <summary>
		/// 获取配置值
		/// </summary>
		/// <typeparam name="T">值类型</typeparam>
		/// <param name="settingElement">强类型配置实例</param>
		/// <returns>配置值</returns>
		public static T GetSetting<T>(this IConfigSettingElement settingElement) {
			return settingElement.GetSetting<T>(null);
		}

		/// <summary>
		/// 获取配置值
		/// </summary>
		/// <param name="settingElement">强类型配置实例</param>
		/// <param name="name">值名</param>
		/// <returns>配置值</returns>
		public static string GetSetting(this IConfigSettingElement settingElement, string name) {
			return settingElement.ConfigSetting.GetSetting(name);
		}

		/// <summary>
		/// 获取配置值集合
		/// </summary>
		/// <typeparam name="T">值类型</typeparam>
		/// <param name="settingElement">强类型配置实例</param>
		/// <param name="name">值名</param>
		/// <returns>配置值集合</returns>
		public static ICollection<T> GetSettings<T>(this IConfigSettingElement settingElement, string name) where T : IConfigSettingElement, new() {
			return settingElement.ConfigSetting.GetSettings<T>(name);
		}

		/// <summary>
		/// 获取配置值集合
		/// </summary>
		/// <typeparam name="T">值类型</typeparam>
		/// <param name="settingElement">强类型配置实例</param>
		/// <param name="name">值名</param>
		/// <param name="initializer">对元素进行初始化</param>
		/// <returns>配置值集合</returns>
		public static ICollection<T> GetSettingsWithInitializer<T>(this IConfigSettingElement settingElement, string name, Action<T> initializer) where T : IConfigSettingElement, new() {
			return settingElement.ConfigSetting.GetSettingsWithInitializer(name, initializer);
		}

		/// <summary>
		/// 获取配置值集合
		/// </summary>
		/// <typeparam name="T">值类型</typeparam>
		/// <param name="settingElement">强类型配置实例</param>
		/// <param name="name">值名</param>
		/// <param name="settingName">集合值名</param>
		/// <returns>配置值集合</returns>
		public static ICollection<T> GetSettings<T>(this IConfigSettingElement settingElement, string name, string settingName) where T : IConfigSettingElement, new() {
			return settingElement.ConfigSetting.GetSettings<T>(name, settingName);
		}

		/// <summary>
		/// 获取配置值集合
		/// </summary>
		/// <typeparam name="T">值类型</typeparam>
		/// <param name="settingElement">强类型配置实例</param>
		/// <param name="name">值名</param>
		/// <param name="settingName">集合值名</param>
		/// <param name="initializer">对元素进行初始化</param>
		/// <returns>配置值集合</returns>
		public static ICollection<T> GetSettingsWithInitializer<T>(this IConfigSettingElement settingElement, string name, string settingName, Action<T> initializer) where T : IConfigSettingElement, new() {
			return settingElement.ConfigSetting.GetSettingsWithInitializer(name, settingName, initializer);
		}

		#endregion IConfigSettingElement Extensions
	
		#region IConfigSettingElement Required Extensions

		/// <summary>
		/// 获取必须配置的值
		/// </summary>
		/// <typeparam name="T">值类型</typeparam>
		/// <param name="settingElement">强类型配置实例</param>
		/// <param name="name">值名</param>
		/// <returns>配置值</returns>
		public static T GetRequiredSetting<T>(this IConfigSettingElement settingElement, string name) {
			return settingElement.ConfigSetting.GetRequiredSetting<T>(name);
		}

		/// <summary>
		/// 获取必须配置的值
		/// </summary>
		/// <typeparam name="T">值类型</typeparam>
		/// <param name="settingElement">强类型配置实例</param>
		/// <returns>配置值</returns>
		public static T GetRequiredSetting<T>(this IConfigSettingElement settingElement) {
			return settingElement.ConfigSetting.GetRequiredSetting<T>();
		}

		/// <summary>
		/// 获取必须配置的值
		/// </summary>
		/// <param name="settingElement">强类型配置实例</param>
		/// <param name="name">值名</param>
		/// <returns>配置值</returns>
		public static string GetRequiredSetting(this IConfigSettingElement settingElement, string name) {
			return settingElement.ConfigSetting.GetRequiredSetting(name);
		}

		/// <summary>
		/// 获取必须配置的值集合
		/// </summary>
		/// <typeparam name="T">值类型</typeparam>
		/// <param name="settingElement">强类型配置实例</param>
		/// <param name="name">值名</param>
		/// <returns>配置值集合</returns>
		public static ICollection<T> GetRequiredSettings<T>(this IConfigSettingElement settingElement, string name) where T : IConfigSettingElement, new() {
			return settingElement.ConfigSetting.GetRequiredSettings<T>(name);
		}

		/// <summary>
		/// 获取配置值集合
		/// </summary>
		/// <typeparam name="T">值类型</typeparam>
		/// <param name="settingElement">强类型配置实例</param>
		/// <param name="name">值名</param>
		/// <param name="initializer">对元素进行初始化</param>
		/// <returns>配置值集合</returns>
		public static ICollection<T> GetRequiredSettingsWithInitializer<T>(this IConfigSettingElement settingElement, string name, Action<T> initializer) where T : IConfigSettingElement, new() {
			return settingElement.ConfigSetting.GetRequiredSettingsWithInitializer(name, initializer);
		}

		/// <summary>
		/// 获取配置值集合
		/// </summary>
		/// <typeparam name="T">值类型</typeparam>
		/// <param name="settingElement">强类型配置实例</param>
		/// <param name="name">值名</param>
		/// <param name="settingName">集合值名</param>
		/// <returns>配置值集合</returns>
		public static ICollection<T> GetRequiredSettings<T>(this IConfigSettingElement settingElement, string name, string settingName) where T : IConfigSettingElement, new() {
			return settingElement.ConfigSetting.GetRequiredSettings<T>(name, settingName);
		}

		/// <summary>
		/// 获取配置值集合
		/// </summary>
		/// <typeparam name="T">值类型</typeparam>
		/// <param name="settingElement">强类型配置实例</param>
		/// <param name="name">值名</param>
		/// <param name="settingName">集合值名</param>
		/// <param name="initializer">对元素进行初始化</param>
		/// <returns>配置值集合</returns>
		public static ICollection<T> GetRequiredSettingsWithInitializer<T>(this IConfigSettingElement settingElement, string name, string settingName, Action<T> initializer) where T : IConfigSettingElement, new() {
			return settingElement.ConfigSetting.GetRequiredSettingsWithInitializer(name, settingName, initializer);
		}

		#endregion IConfigSettingElement Extensions

		#region Cached IConfigSettingElement Extensions

		/// <summary>
		/// 获取配置值
		/// </summary>
		/// <typeparam name="T">值类型</typeparam>
		/// <param name="settingElement">强类型配置实例</param>
		/// <param name="name">值名</param>
		/// <param name="defaultValue">值缺省值</param>
		/// <returns>配置值</returns>
		public static T GetCachedSetting<T>(this IConfigSettingElement settingElement, string name, T defaultValue) {
			return settingElement.GetObjectContext(name, () => settingElement.GetSetting(name, defaultValue));
		}

		/// <summary>
		/// 获取配置值
		/// </summary>
		/// <typeparam name="T">值类型</typeparam>
		/// <param name="settingElement">强类型配置实例</param>
		/// <param name="name">值名</param>
		/// <returns>配置值</returns>
		public static T GetCachedSetting<T>(this IConfigSettingElement settingElement, string name) {
			return settingElement.GetObjectContext(name, () => settingElement.GetSetting<T>(name));
		}

		/// <summary>
		/// 获取配置值
		/// </summary>
		/// <typeparam name="T">值类型</typeparam>
		/// <param name="settingElement">强类型配置实例</param>
		/// <returns>配置值</returns>
		public static T GetCachedSetting<T>(this IConfigSettingElement settingElement) {
			return settingElement.GetObjectContext(typeof(IConfigSettingElement), () => settingElement.GetSetting<T>());
		}

		/// <summary>
		/// 获取配置值
		/// </summary>
		/// <param name="settingElement">强类型配置实例</param>
		/// <param name="name">值名</param>
		/// <returns>配置值</returns>
		public static string GetCachedSetting(this IConfigSettingElement settingElement, string name) {
			return settingElement.GetObjectContext(name, () => settingElement.GetSetting(name));
		}

		/// <summary>
		/// 获取配置值集合
		/// </summary>
		/// <typeparam name="T">值类型</typeparam>
		/// <param name="settingElement">强类型配置实例</param>
		/// <param name="name">值名</param>
		/// <returns>配置值集合</returns>
		public static T[] GetCachedSettings<T>(this IConfigSettingElement settingElement, string name) where T : IConfigSettingElement, new() {
			return settingElement.GetObjectContext(name, () => settingElement.GetSettings<T>(name).ToArray());
		}

		/// <summary>
		/// 获取配置值集合
		/// </summary>
		/// <typeparam name="T">值类型</typeparam>
		/// <param name="settingElement">强类型配置实例</param>
		/// <param name="name">值名</param>
		/// <param name="initializer">对元素进行初始化</param>
		/// <returns>配置值集合</returns>
		public static T[] GetCachedSettingsWithInitializer<T>(this IConfigSettingElement settingElement, string name, Action<T> initializer) where T : IConfigSettingElement, new() {
			return settingElement.GetObjectContext(name, () => settingElement.GetSettingsWithInitializer(name, initializer).ToArray());
		}

		/// <summary>
		/// 获取配置值集合
		/// </summary>
		/// <typeparam name="T">值类型</typeparam>
		/// <param name="settingElement">强类型配置实例</param>
		/// <param name="name">值名</param>
		/// <param name="settingName">集合值名</param>
		/// <returns>配置值集合</returns>
		public static T[] GetCachedSettings<T>(this IConfigSettingElement settingElement, string name, string settingName) where T : IConfigSettingElement, new() {
			return settingElement.GetObjectContext(name, () => settingElement.GetSettings<T>(name, settingName).ToArray());
		}

		/// <summary>
		/// 获取配置值集合
		/// </summary>
		/// <typeparam name="T">值类型</typeparam>
		/// <param name="settingElement">强类型配置实例</param>
		/// <param name="name">值名</param>
		/// <param name="settingName">集合值名</param>
		/// <param name="initializer">对元素进行初始化</param>
		/// <returns>配置值集合</returns>
		public static T[] GetCachedSettingsWithInitializer<T>(this IConfigSettingElement settingElement, string name, string settingName, Action<T> initializer) where T : IConfigSettingElement, new() {
			return settingElement.GetObjectContext(name, () => settingElement.GetSettingsWithInitializer(name, settingName, initializer).ToArray());
		}

		#endregion
	}
}

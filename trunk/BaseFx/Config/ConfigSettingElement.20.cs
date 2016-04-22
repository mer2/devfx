/* Copyright(c) 2005-2011 R2@DevFx.NET, License(LGPL) */

using System;
using System.Collections.Generic;

namespace HTB.DevFx.Config
{
	partial class ConfigSettingElement
	{
		/// <summary>
		/// 获取配置值
		/// </summary>
		/// <param name="name">值名</param>
		/// <returns>配置值</returns>
		public string GetSetting(string name) {
			return Config.ConfigSetting.GetSetting(this.ConfigSetting, name);
		}

		/// <summary>
		/// 获取配置值
		/// </summary>
		/// <param name="name">值名</param>
		/// <param name="defaultValue">缺省值</param>
		/// <returns>配置值</returns>
		public T GetSetting<T>(string name, T defaultValue) {
			var settingValue = Config.ConfigSetting.GetSettingValue(this.ConfigSetting, name);
			return settingValue == null ? defaultValue : settingValue.TryToObject(defaultValue);
		}

		/// <summary>
		/// 获取配置实例
		/// </summary>
		/// <typeparam name="T">配置类型</typeparam>
		/// <param name="xpath">路径</param>
		/// <returns>配置实例</returns>
		public T GetTypedSetting<T>(string xpath) where T : IConfigSettingElement, new() {
			return string.IsNullOrEmpty(xpath) ? Config.ConfigSetting.ToSetting<T>(this.ConfigSetting) : Config.ConfigSetting.ToSetting<T>(this.ConfigSetting, xpath);
		}

		/// <summary>
		/// 获取配置值集合
		/// </summary>
		/// <typeparam name="T">值类型</typeparam>
		/// <param name="name">子节名</param>
		/// <returns>配置值集合</returns>
		public List<T> GetSettings<T>(string name) where T : IConfigSettingElement, new() {
			var list = new List<T>();
			var setting = this.ConfigSetting;
			if (setting != null) {
				var settings = string.IsNullOrEmpty(name) ? setting.GetChildSettings() : setting.GetChildSettings(name);
				foreach(var item in settings) {
					var t = Config.ConfigSetting.ToSetting<T>(item);
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
		public List<T> GetSettings<T>(string name, string settingName) where T : IConfigSettingElement, new() {
			var list = new List<T>();
			var setting = this.ConfigSetting;
			if(setting != null) {
				setting = this.ConfigSetting[name];
			}
			if (setting != null) {
				var settings = string.IsNullOrEmpty(settingName) ? setting.GetChildSettings() : setting.GetChildSettings(settingName);
				foreach(var item in settings) {
					var t = Config.ConfigSetting.ToSetting<T>(item);
					list.Add(t);
				}
			}
			return list;
		}
	}
}

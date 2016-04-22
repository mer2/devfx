/* Copyright(c) 2005-2011 R2@DevFx.NET, License(LGPL) */

namespace HTB.DevFx.Config
{
	partial class ConfigSetting
	{	
		/// <summary>
		/// 获取配置值
		/// </summary>
		/// <param name="configSetting">配置节</param>
		/// <param name="name">名称</param>
		/// <returns>配置值</returns>
		public static ISettingValue GetSettingValue(IConfigSetting configSetting, string name) {
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
		/// <param name="configSetting">配置节</param>
		/// <param name="name">值名</param>
		/// <returns>配置值</returns>
		public static string GetSetting(IConfigSetting configSetting, string name) {
			var settingValue = GetSettingValue(configSetting, name);
			return settingValue == null ? null : settingValue.Value;
		}

		/// <summary>
		/// 把配置节转化成强类型配置类
		/// </summary>
		/// <typeparam name="T">强类型配置类型</typeparam>
		/// <param name="configSetting">配置节</param>
		/// <returns>强类型配置实例</returns>
		public static T ToSetting<T>(IConfigSetting configSetting) where T : IConfigSettingElement, new() {
			return configSetting == null ? default(T) : new T { ConfigSetting = configSetting };
		}

		/// <summary>
		/// 把配置节转化成强类型配置类
		/// </summary>
		/// <typeparam name="T">强类型配置类型</typeparam>
		/// <param name="configSetting">配置节</param>
		/// <param name="xpath">需被转换的配置节路径</param>
		/// <returns>强类型配置实例</returns>
		public static T ToSetting<T>(IConfigSetting configSetting, string xpath) where T : IConfigSettingElement, new() {
			if (configSetting != null) {
				var setting = configSetting.GetChildSetting(xpath);
				if (setting != null) {
					return new T { ConfigSetting = setting };
				}
			}
			return default(T);
		}
	}
}

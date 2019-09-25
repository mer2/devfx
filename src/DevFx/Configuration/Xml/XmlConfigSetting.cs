/* Copyright(c) 2005-2011 R2@DevFx.NET, License(LGPL) */

using DevFx.Utils;
using System.Collections.Generic;
using System.IO;
using System.Xml;

namespace DevFx.Configuration.Xml
{
	/// <summary>
	/// XML方式实现配置节 <see cref="ConfigSetting"/>
	/// </summary>
	public class XmlConfigSetting : ConfigSetting
	{
		#region constructor

		/// <summary>
		/// 构造方法
		/// </summary>
		protected XmlConfigSetting() {}

		#endregion

		/// <summary>
		/// 创建配置节实例
		/// </summary>
		/// <returns></returns>
		protected override ConfigSetting CreateConfigSetting() {
			return new XmlConfigSetting();
		}

		/// <summary>
		/// 创建配置值
		/// </summary>
		/// <param name="name">配置值名</param>
		/// <param name="value">配置值</param>
		/// <param name="readonly">是否只读</param>
		/// <param name="values">多值</param>
		/// <returns>SettingValue</returns>
		protected override SettingValue CreateSettingValue(string name, string value, bool @readonly, object[] values) {
			return new XmlSettingValue(name, value, @readonly, values);
		}

		/// <summary>
		/// 创建配置属性实例
		/// </summary>
		/// <param name="readonly">是否只读</param>
		/// <returns>SettingProperty</returns>
		protected override SettingProperty CreateSettingProperty(bool @readonly) {
			return new XmlSettingProperty(@readonly);
		}

		/// <summary>
		/// 创建配置节
		/// </summary>
		/// <param name="parent">父配置节</param>
		/// <param name="xmlNode">XML节</param>
		/// <param name="readonly">是否只读</param>
		/// <param name="searchPath">XML搜索目录列表</param>
		/// <param name="configFiles">如果有子配置文件，则添加到此列表</param>
		internal static XmlConfigSetting Create(XmlConfigSetting parent, XmlNode xmlNode, bool @readonly, string[] searchPath, List<string> configFiles) {
			if(xmlNode.NodeType != XmlNodeType.Element) {
				throw new ConfigException("解析到非法元素");
			}

			if(searchPath == null || searchPath.Length <= 0) {
				searchPath = ConfigHelper.ConfigFileDefaultSearchPath;
			}
			var setting = new XmlConfigSetting {
				parent = parent,
				ReadOnly = @readonly,
				property = new XmlSettingProperty(xmlNode, @readonly),
				childSettings = new ConfigSettingCollection(true),
				operatorSettings = new ConfigSettingCollection(false),
				value = new XmlSettingValue(xmlNode, @readonly)
			};
			setting.SettingName = setting.Name;
			var settingOperator = setting.SettingOperator;
			if (settingOperator != 0 && settingOperator != ConfigSettingOperator.Clear) {
				var operatorKey = setting.ConfigOperatorKey;
				var newName = setting.Property.TryGetPropertyValue(operatorKey);
				if(string.IsNullOrEmpty(newName) && !setting.ConfigKeyNullable) {
					throw new ConfigException("配置命令未定义属性：" + operatorKey);
				}
				setting.value.SetName(newName);
			}

			foreach(XmlNode node in xmlNode.ChildNodes) {
				if(node.NodeType != XmlNodeType.Element) {
					continue;
				}
				var childSetting = Create(setting, node, @readonly, searchPath, configFiles);
				setting.operatorSettings.Add(childSetting);
			}
			Compile(setting, setting.operatorSettings);

			var sourceDone = false;
			CheckConfigProvider(setting, ref sourceDone);

			var configFile = setting.ConfigFile;
			var configNode = setting.ConfigNode;
			if(!sourceDone && !string.IsNullOrEmpty(configFile)) {
				configFile = ConfigHelper.SearchConfigFile(configFile, searchPath);
				if(!string.IsNullOrEmpty(configFile)) {
					if(string.IsNullOrEmpty(configNode)) {
						configNode = "/";
					}
					var newNode = ConfigHelper.LoadXmlNodeFromFile(configFile, configNode, false);
					if(newNode != null) {
						configFiles?.Add(configFile);
						setting.Merge(Create(parent, newNode, @readonly, searchPath, configFiles));
					}
				}
			}
			return setting;
		}

		private static void CheckConfigProvider(ConfigSetting setting, ref bool sourceDone) {
			var configProvider = setting.ConfigProvider;
			if (!string.IsNullOrEmpty(configProvider)) {
				var providerType = TypeHelper.CreateType(configProvider, false);
				if (providerType != null) {
					if (TypeHelper.TryInvoke(providerType, "GetConfigSetting", out var returnValue, false, setting.ConfigSource)) {
						if (returnValue is ConfigSetting configSetting) {
							setting.Merge(configSetting);
							sourceDone = true;
						}
					}
				}
			}
		}

		/// <summary>
		/// 创建配置节
		/// </summary>
		/// <param name="fileName">XML文件</param>
		/// <param name="readonly">是否只读</param>
		/// <param name="searchPath">XML搜索目录列表</param>
		/// <param name="configFiles">如果有子配置文件，则添加到此列表</param>
		/// <returns>配置节</returns>
		internal static XmlConfigSetting Create(string fileName, bool @readonly, string[] searchPath, List<string> configFiles) {
			fileName = ConfigHelper.SearchConfigFile(fileName, searchPath);
			if(string.IsNullOrEmpty(fileName)) {
				return null;
			}
			var xmlNode = ConfigHelper.LoadXmlNodeFromFile(fileName, "/", false);
			if(xmlNode == null) {
				return null;
			}
			if(searchPath == null || searchPath.Length <= 0) {
				searchPath = ConfigHelper.ConfigFileDefaultSearchPath;
			}
			var newSearchPath = new string[searchPath.Length + 1];
			newSearchPath[0] = Path.GetDirectoryName(fileName);
			searchPath.CopyTo(newSearchPath, 1);
			return Create(null, xmlNode, @readonly, newSearchPath, configFiles);
		}

		/// <summary>
		/// 创建配置节
		/// </summary>
		/// <param name="fileName">XML文件</param>
		/// <param name="readonly">是否只读</param>
		/// <returns>配置节</returns>
		internal static XmlConfigSetting Create(string fileName, bool @readonly) {
			return Create(fileName, @readonly, ConfigHelper.ConfigFileDefaultSearchPath, null);
		}

		/// <summary>
		/// 创建配置节
		/// </summary>
		/// <param name="fileName">XML文件</param>
		/// <returns>配置节</returns>
		internal static XmlConfigSetting Create(string fileName) {
			return Create(fileName, true);
		}
	}
}
/* Copyright(c) 2005-2011 R2@DevFx.NET, License(LGPL) */

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Xml;
using DevFx.Configuration.Xml;
using DevFx.Utils;

namespace DevFx.Configuration
{
	/// <summary>
	/// 关于配置的一些实用方法
	/// </summary>
	public static class ConfigHelper
	{
		public const string RootSettingName = "devfx";

		/// <summary>
		/// 缺省的XML配置文件查找目录列表
		/// </summary>
		public static readonly string[] ConfigFileDefaultSearchPath = {
			"./", AppDomain.CurrentDomain.BaseDirectory
		};

		/// <summary>
		/// 验证配置文件路径并返回
		/// </summary>
		/// <param name="fileName">配置文件名</param>
		/// <param name="searchPath">搜索目录列表</param>
		/// <returns>返回配置文件路径</returns>
		public static string SearchConfigFile(string fileName, string[] searchPath) {
			if(searchPath == null || searchPath.Length <= 0) {
				searchPath = ConfigFileDefaultSearchPath;
			}
			return FileHelper.SearchFile(fileName, searchPath);
		}

		/// <summary>
		/// 查找由通配符指定的文件集合
		/// </summary>
		/// <param name="filePattern">文件通配符</param>
		/// <param name="searchPath">搜索目录列表</param>
		/// <returns>找到的文件列表</returns>
		public static string[] SearchConfigFileWithPattern(string filePattern, string[] searchPath) {
			if(searchPath == null || searchPath.Length <= 0) {
				searchPath = ConfigFileDefaultSearchPath;
			}
			return FileHelper.SearchFileWithPattern(filePattern, searchPath);
		}

		/// <summary>
		/// 从Xml字符串中生成 <see cref="IConfigSetting"/>
		/// </summary>
		/// <param name="xmlString">Xml字符串</param>
		/// <returns><see cref="IConfigSetting"/></returns>
		public static IConfigSetting CreateFromXmlString(string xmlString) {
			var xmlNode = LoadXmlNodeFromString(xmlString, "/");
			if(xmlNode is XmlDocument) {
				xmlNode = ((XmlDocument)xmlNode).DocumentElement;
			}
			return XmlConfigSetting.Create(null, xmlNode, true, null, null);
		}

		/// <summary>
		/// 从Xml文件中生成 <see cref="IConfigSetting"/>
		/// </summary>
		/// <param name="xmlFileName">Xml文件</param>
		/// <returns><see cref="IConfigSetting"/></returns>
		public static IConfigSetting CreateFromXmlFile(string xmlFileName) {
			return XmlConfigSetting.Create(xmlFileName);
		}

		/// <summary>
		/// 从 <see cref="XmlNode"/> 生成 <see cref="IConfigSetting"/>
		/// </summary>
		/// <param name="xmlNode"><see cref="XmlNode"/></param>
		/// <returns><see cref="IConfigSetting"/></returns>
		public static IConfigSetting CreateFromXmlNode(XmlNode xmlNode) {
			return XmlConfigSetting.Create(null, xmlNode, true, null, null);
		}

		/// <summary>
		/// 从资源（Uri）中生成 <see cref="IConfigSetting"/>
		/// </summary>
		/// <param name="xmlSource">Uri字符串</param>
		/// <param name="sourceInType">如果是内嵌资源则表示其所在的程序集</param>
		/// <returns><see cref="IConfigSetting"/></returns>
		public static IConfigSetting CreateFromXmlSource(string xmlSource, Type sourceInType) {
			return CreateFromXmlSource(xmlSource, sourceInType.Assembly);
		}

		/// <summary>
		/// 从资源（Uri）中生成 <see cref="IConfigSetting"/>
		/// </summary>
		/// <param name="sourceName">资源的全名称</param>
		/// <param name="sourceAssembly">如果是内嵌资源则表示其所在的程序集</param>
		/// <returns><see cref="IConfigSetting"/></returns>
		public static IConfigSetting CreateFromXmlSource(string sourceName, Assembly sourceAssembly) {
			IConfigSetting setting;
			if (sourceName.StartsWith("res://", true, null)) {
				sourceName = FileHelper.GetResourceName(sourceName, out _);
				var stream = sourceAssembly.GetManifestResourceStream(sourceName);
				if (stream == null) {
					throw new ConfigException("未找到资源" + sourceName);
				}
				var sr = new StreamReader(stream);
				var xmlString = sr.ReadToEnd();
				setting = CreateFromXmlString(xmlString);
			} else if (sourceName.StartsWith("http://", true, null)) {
				throw new ConfigException("未实现http://");
			} else {
				setting = CreateFromXmlFile(sourceName);
			}
			return setting;
		}

		/// <summary>
		/// 从资源（Uri）中生成 <see cref="IConfigSetting"/>
		/// </summary>
		/// <param name="xmlSource">Uri字符串（如果是内嵌资源，则此Uri还应包含所在程序集的名称，形如：res://内嵌资源全名称, 所在程序集名称）</param>
		/// <returns><see cref="IConfigSetting"/></returns>
		public static IConfigSetting CreateFromXmlSource(string xmlSource) {
			var stream = FileHelper.GetFileStream(xmlSource);
			if (stream == null) {
				throw new ConfigException("未找到资源" + xmlSource);
			}
			var sr = new StreamReader(stream);
			var xmlString = sr.ReadToEnd();
			return CreateFromXmlString(xmlString);
		}

		/// <summary>
		/// 创建无内容的配置节
		/// </summary>
		/// <returns>配置节</returns>
		public static IConfigSetting CreateEmptySetting() {
			return CreateFromXmlString("<configuration />");
		}

		/// <summary>
		/// 获取XML文件的内容
		/// </summary>
		/// <param name="fileName">XML文件名</param>
		/// <param name="sectionName">对应的XPath</param>
		/// <param name="rawType">是否不进行任何转换而返回</param>
		/// <returns>XmlNode</returns>
		public static XmlNode LoadXmlNodeFromFile(string fileName, string sectionName, bool rawType) {
			var doc = new XmlDocument();
			LoadXmlFile(doc, fileName);
			var xmlNode = doc.SelectSingleNode(sectionName);
			xmlNode = xmlNode?.CloneNode(true);
			if(!rawType && xmlNode is XmlDocument) {
				xmlNode = ((XmlDocument)xmlNode).DocumentElement;
			}
			return xmlNode;
		}

		/// <summary>
		/// 分析XML字符串内容
		/// </summary>
		/// <param name="xmlString">XML字符串</param>
		/// <param name="sectionName">对应的XPath</param>
		/// <returns>XmlNode</returns>
		public static XmlNode LoadXmlNodeFromString(string xmlString, string sectionName) {
			var doc = new XmlDocument();
			doc.LoadXml(xmlString);
			var xmlNode = doc.SelectSingleNode(sectionName);
			return xmlNode?.CloneNode(true);
		}

		internal static IConfigSetting LoadConfigSettingFromAssemblies(Assembly coreAssembly, List<Assembly> assemblies, IConfigSetting appSetting, IConfigSetting userRootSetting, string[] resnamePatterns = null) {
			return LoadConfigSettingFromAssemblies(new []{ coreAssembly }, assemblies, appSetting, userRootSetting, resnamePatterns);
		}

		internal static IConfigSetting LoadConfigSettingFromAssemblies(Assembly[] coreAssemblies, List<Assembly> assemblies, IConfigSetting appSetting, IConfigSetting userRootSetting, string[] resnamePatterns = null) {
			if(assemblies == null) {
				assemblies = new List<Assembly>();
			}

			var theAssembly = typeof(ObjectService).Assembly;
			var theName = theAssembly.FullName;
			var refAssemblies = TypeHelper.FindAssemblyChildren(theAssembly);//仅处理有引用本类库的程序集/或处理本类库

			if(assemblies == null) {
				assemblies = refAssemblies;
			} else {
				assemblies = TypeHelper.SortAssemblies(assemblies, refAssemblies);
				//把核心Assembly放在最前面
				for (var i = coreAssemblies.Length - 1; i >= 0; i--) {
					var coreAssembly = coreAssemblies[i];
					assemblies.Remove(coreAssembly);
					assemblies.Insert(0, coreAssembly);
				}
			}

			var resources = new List<ConfigResource>();
			//获取由ConfigResourceAttribute定义的资源文件
			foreach (var assembly in assemblies) {
				var attributes = assembly.GetCustomAttributes(typeof(ConfigResourceAttribute), true) as ConfigResourceAttribute[];
				if (attributes != null && attributes.Length > 0) {
					var attributeList = new List<ConfigResourceAttribute>(attributes);
					attributeList.Sort((x, y) => x.Index - y.Index);
					foreach (var attribute in attributeList) {
						if (!resources.Exists(x => x.ResourceName == attribute.Resource && x.Assembly == assembly)) {
							resources.Add(new ConfigResource {  ResourceName = attribute.Resource, Assembly = assembly });
						}
					}
				}
			}

			//获取由resnamePatterns指定的资源
			if(resnamePatterns != null && resnamePatterns.Length > 0) {
				foreach(var ass in assemblies) {
					if(ass.IsDynamic) {
						continue;
					}
					var names = ass.GetManifestResourceNames();
					if(names != null && names.Length > 0) {
						var nameList = names.Where(x => resnamePatterns.Any(y => y != null && x.EndsWith(y)));
						foreach(var name in nameList) {
							if (!resources.Exists(x => x.ResourceName == name && x.Assembly == ass)) {
								resources.Add(new ConfigResource { ResourceName = "res://" + name, Assembly = ass });
							}
						}
					}
				}
			}

			//创建并合并这些资源文件
			var rootSetting = CreateFromXmlString($"<configuration><{RootSettingName} /></configuration>");
			foreach (var resource in resources) {
				var configSetting = CreateFromXmlSource(resource.ResourceName, resource.Assembly);
				rootSetting.Merge(configSetting);
			}

			if (userRootSetting != null) {
				userRootSetting = userRootSetting.GetRootSetting();
			}
			if (userRootSetting == null) {
				userRootSetting = appSetting;
			}

			if (userRootSetting != null) {
				var mergeSetting = rootSetting;
				if (userRootSetting.SettingName == RootSettingName) {
					mergeSetting = rootSetting[RootSettingName];
				}
				mergeSetting?.Merge(userRootSetting);
			}
			return rootSetting;
		}

		/// <summary>
		/// 载入XML文件内容
		/// </summary>
		/// <param name="doc">XmlDocument</param>
		/// <param name="fileName">文件名</param>
		private static void LoadXmlFile(XmlDocument doc, string fileName) {
			doc.Load(fileName);
		}

		private class ConfigResource
		{
			public string ResourceName { get; set; }
			public Assembly Assembly { get; set; }
		}
	}
}
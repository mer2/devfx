/* Copyright(c) 2005-2011 R2@DevFx.NET, License(LGPL) */

using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Reflection;
using System.Web;
using System.Web.Configuration;
using System.Xml;
using HTB.DevFx.Config.DotNetConfig;
using HTB.DevFx.Config.XmlConfig;
using HTB.DevFx.Utils;

namespace HTB.DevFx.Config
{
	/// <summary>
	/// 关于配置的一些实用方法
	/// </summary>
	public static class ConfigHelper
	{
		/// <summary>
		/// 缺省的XML配置文件查找目录列表
		/// </summary>
		public static readonly string[] ConfigFileDefaultSearchPath = {
			"./", "./../", "./../../", "./../../../",
			"./Configuration/", "./../Configuration/", "./../../Configuration/", "./../../../Configuration/",
			Environment.CurrentDirectory + "/",
			AppDomain.CurrentDomain.SetupInformation.ApplicationBase
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
				string assemblyName;
				sourceName = FileHelper.GetResourceName(sourceName, out assemblyName);
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
			if(xmlNode != null) {
				xmlNode = xmlNode.CloneNode(true);
			}
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
			return xmlNode != null ? xmlNode.CloneNode(true) : null;
		}

		/// <summary>
		/// 从配置文件配置节中获取配置节实例
		/// </summary>
		/// <typeparam name="T">配置节类型</typeparam>
		/// <param name="createDefault">如果没有配置，是否创建缺省实例</param>
		/// <param name="sectionNames">配置节名列表（找到为止）</param>
		/// <returns>配置节实例</returns>
		public static T GetSectionFromConfiguration<T>(bool createDefault, params string[] sectionNames) where T : class {
			T section = null;
			if (sectionNames != null && sectionNames.Length > 0) {
				var isHttp = HttpContext.Current != null;
				foreach (var sectionName in sectionNames) {
					if (isHttp) {
						section = WebConfigurationManager.GetSection(sectionName) as T;
					} else {
						section = ConfigurationManager.GetSection(sectionName) as T;
					}
					if (section != null) {
						break;
					}
				}
			}
			var t = typeof(T);
			if (section == null && (createDefault || typeof(IRequiresEmptyInstance).IsAssignableFrom(t))) {
				section = (T)TypeHelper.CreateObject(t, t, true);
			}
			return section;
		}

		/// <summary>
		/// 从配置文件配置节中获取配置节实例
		/// </summary>
		/// <param name="sectionNames">配置节名列表（找到为止）</param>
		/// <returns>配置节实例</returns>
		public static object GetSectionFromConfiguration(params string[] sectionNames) {
			object section = null;
			if (sectionNames != null && sectionNames.Length > 0) {
				var isHttp = HttpContext.Current != null;
				foreach (var sectionName in sectionNames) {
					section = isHttp ? WebConfigurationManager.GetSection(sectionName) : ConfigurationManager.GetSection(sectionName);
					if (section != null) {
						break;
					}
				}
			}
			return section;
		}

		/// <summary>
		/// 对Assembly进行依赖排序
		/// </summary>
		/// <param name="list">排序后的结果</param>
		/// <param name="assemblies">需要进行排序的Assembly</param>
		/// <returns>排序后的结果</returns>
		internal static List<Assembly> SortAssemblies(List<Assembly> list, IEnumerable<Assembly> assemblies) {
			//根据程序集逻辑依赖进行排序
			foreach (var assembly in assemblies) {
				if (!list.Contains(assembly)) {
					list.Add(assembly);
				}
				var index = list.IndexOf(assembly);
				var assemblyNames = assembly.GetReferencedAssemblies();
				var referenced = new List<Assembly>();
				foreach (var assemblyName in assemblyNames) {
					var ass = TypeHelper.AssemblyLoad(assemblyName, false);
					if (ass == null || !ass.IsDefined(typeof(ConfigResourceAttribute), false)) {
						continue;
					}
					var assIndex = list.IndexOf(ass);
					if (assIndex > index) {
						list.Remove(ass);
					}
					if (!list.Contains(ass) && !referenced.Contains(ass)) {
						referenced.Add(ass);
					}
				}
				if (referenced.Count > 0) {
					list.InsertRange(index, referenced);
				}
			}
			//根据程序集用户指定的依赖关系排序
			var sortedList = new List<Assembly>();
			for(var i = 0; i < list.Count;) {
				var assembly = list[i];
				var hasDependency = false;
				if(sortedList.Contains(assembly)) {//已经排序过了，直接跳过
					goto CONTINUE;
				}
				if(!assembly.IsDefined(typeof(ConfigDependencyAttribute), false)) {//没有此属性，直接跳过
					goto CONTINUE;
				}
				var attributes = assembly.GetCustomAttributes(typeof(ConfigDependencyAttribute), false) as ConfigDependencyAttribute[];
				if(attributes == null || attributes.Length <= 0) {//没有此属性，直接跳过
					goto CONTINUE;
				}
				//获取依赖属性
				var sortedAttributes = new List<ConfigDependencyAttribute>(attributes);
				//排序
				sortedAttributes.Sort(CompareConfigDependencyAttributeByIndex);
				sortedAttributes.Reverse();
				//把依赖程序集放到本程序集前
				foreach(var dependencyAttribute in sortedAttributes) {
					var assName = dependencyAttribute.AssemblyName;
					if(string.IsNullOrEmpty(assName)) {
						continue;
					}
					var dependencyAssembly = TypeHelper.AssemblyLoad(assName, false);
					if(dependencyAssembly == null) {
						continue;
					}
					var dependencyIndex = list.IndexOf(dependencyAssembly);
					if(dependencyIndex <= i) {//不存在或已经在本程序集前了，不处理
						continue;
					}
					//把依赖程序集放到本程序集前
					list.Remove(dependencyAssembly);
					list.Insert(i, dependencyAssembly);
					hasDependency = true;
				}
				CONTINUE:
				if(!hasDependency) {//没有依赖，则移动处理下一个
					i++;
				}
				if(!sortedList.Contains(assembly)) {//把已处理过的标记下，下次就不用再处理了
					sortedList.Add(assembly);
				}
			}
			return list;
		}

		private static int CompareConfigResourceAttributeByIndex(ConfigResourceAttribute x, ConfigResourceAttribute y) {
			return x.Index - y.Index;
		}

		private static int CompareConfigDependencyAttributeByIndex(ConfigDependencyAttribute x, ConfigDependencyAttribute y) {
			return x.Index - y.Index;
		}

		internal static IConfigSetting LoadConfigSettingFromAssemblies(Assembly coreAssembly, List<Assembly> assemblies, IConfigSetting appSetting, IConfigSettingRequired setting) {
			return LoadConfigSettingFromAssemblies(new []{ coreAssembly }, assemblies, appSetting, setting);
		}

		internal static IConfigSetting LoadConfigSettingFromAssemblies(Assembly[] coreAssemblies, List<Assembly> assemblies, IConfigSetting appSetting, IConfigSettingRequired setting) {
			if (appSetting == null) {
				appSetting = ConfigSectionHandler.GetConfig("htb.devfx");
			}

			if(assemblies == null) {
				assemblies = new List<Assembly>();
			}

			assemblies = SortAssemblies(assemblies, TypeHelper.GetAssembliesByCustomAttribute(typeof(ConfigResourceAttribute)));
			for (var i = coreAssemblies.Length - 1; i >= 0; i--) {
				var coreAssembly = coreAssemblies[i];
				if (assemblies.IndexOf(coreAssembly) != 0) {
					assemblies.Remove(coreAssembly);
					assemblies.Insert(0, coreAssembly);
				}
			}

			var resources = new List<ConfigResourceAttribute>();
			foreach (var assembly in assemblies) {
				var attributes = assembly.GetCustomAttributes(typeof(ConfigResourceAttribute), true) as ConfigResourceAttribute[];
				if (attributes != null && attributes.Length > 0) {
					foreach (var attribute in attributes) {
						attribute.Assembly = assembly;
					}
					var attributeList = new List<ConfigResourceAttribute>(attributes);
					attributeList.Sort(CompareConfigResourceAttributeByIndex);
					resources.AddRange(attributeList);
				}
			}

			var rootSetting = CreateFromXmlString("<configuration><htb.devfx /></configuration>");
			foreach (var resource in resources) {
				var configSetting = CreateFromXmlSource(resource.Resource, resource.Assembly);
				rootSetting.Merge(configSetting);
			}

			IConfigSetting userRootSetting = null;
			if (setting != null && setting.ConfigSetting != null) {
				userRootSetting = setting.ConfigSetting.GetRootSetting();
			}
			if (userRootSetting == null) {
				userRootSetting = appSetting;
			}

			if (userRootSetting != null) {
				var mergeSetting = rootSetting;
				if (userRootSetting.SettingName == "htb.devfx") {
					mergeSetting = rootSetting["htb.devfx"];
				}
				if (mergeSetting != null) {
					mergeSetting.Merge(userRootSetting);
				}
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
	}
}
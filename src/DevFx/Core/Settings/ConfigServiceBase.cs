/* Copyright(c) 2005-2011 R2@DevFx.NET, License(LGPL) */

using DevFx.Configuration;
using DevFx.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace DevFx.Core.Settings
{
	internal abstract class ConfigServiceBase
	{
		public static IConfigSetting Init(StartupSetting startupSetting, IConfigSetting appSetting) {
			var setting = startupSetting;
			var assemblies = new List<Assembly>();

			//处理需要被明确加载的Assembly
			var assemblySettings = setting?.Assemblies;
			if (assemblySettings != null && assemblySettings.Length > 0) {
				foreach (var assembly in assemblySettings) {
					var dll = TypeHelper.AssemblyLoad(assembly.Name, false);
					if (dll != null && !assemblies.Contains(dll)) {
						assemblies.Add(dll);
					}
				}
			}

			//获取内嵌资源的文件格式
			var resnamePatterns = new List<string> { ".devfx", ".devfx.config", ".devfxconfig", ".sqlconfig" };
			var resourceSettings = setting?.ResourceFiles;
			if (resourceSettings != null && resourceSettings.Length > 0) {
				foreach (var resource in resourceSettings) {
					resnamePatterns.Add(resource.Name);
				}
			}

			var rootSetting = ConfigHelper.LoadConfigSettingFromAssemblies(new[] { typeof(ConfigServiceBase).Assembly }, assemblies, appSetting, setting?.ConfigSetting, resnamePatterns.ToArray());
			setting = rootSetting.ToSetting<StartupSetting>($"{ConfigHelper.RootSettingName}/startup");
			var fileSettings = setting?.ConfigFiles;
			if(fileSettings != null && fileSettings.Length > 0) {
				var configFiles = new List<string>();
				foreach(var fileSetting in fileSettings) {
					var files = FileHelper.SearchFileWithPattern(fileSetting.Name, SearchOption.AllDirectories, AppDomain.CurrentDomain.BaseDirectory);
					if (files != null && files.Length > 0) {
						configFiles.AddRange(files);
					}
				}
				foreach (var fileName in configFiles) {
					var configSetting = ConfigHelper.CreateFromXmlFile(fileName);
					rootSetting.Merge(configSetting);
				}
			}

			ConfigSettingInherit(rootSetting[ConfigHelper.RootSettingName], new List<IConfigSetting>(), new List<IConfigSetting>());

			var debugSetting = setting?.Debug;
			if(debugSetting != null && debugSetting.Enabled) {
				var fileName = debugSetting.FileName;
				if(!string.IsNullOrEmpty(fileName)) {
					fileName = FileHelper.GetPhysicalPath(".", fileName, true);
					File.WriteAllText(fileName, rootSetting.ToString());
				}
			}

			return rootSetting;
		}

		internal sealed class ConfigInheritInternal
		{
			public string Base { get; set; }
			public bool Recursive { get; set; }

			public static ConfigInheritInternal CreateFromJson(string json) {
				var configInherit = WebHelper.FromJsonLite<ConfigInheritInternal>(json, (c, n, v) => {
					n = n.ToLowerInvariant();
					switch(n) {
						case "base":
							c.Base = v;
							break;
						case "recursive":
							c.Recursive = bool.Parse(v);
							break;
					}
				});
				return configInherit;
			}
		}
		private const string ConfigInheritPropertyName = "configInherit";
		private static void ConfigSettingInherit(IConfigSetting configSetting, ICollection<IConfigSetting> loopList, ICollection<IConfigSetting> doneList) {
			var inheritPath = configSetting?.Property.TryGetPropertyValue(ConfigInheritPropertyName);
			if(string.IsNullOrEmpty(inheritPath)) {
				return;
			}
			var recursive = false;
			var inherit = false;
			if(!inheritPath.StartsWith("..")) {
				if(!bool.TryParse(inheritPath, out recursive)) {
					if(inheritPath.StartsWith("{") && inheritPath.EndsWith("}")) {
						var configInherit = ConfigInheritInternal.CreateFromJson(inheritPath);
						if(configInherit != null) {
							inherit = true;
							inheritPath = configInherit.Base;
							recursive = configInherit.Recursive;
						}
					}
				}
			} else {
				inherit = true;
			}
			if (!inherit && !recursive) {
				return;
			}
			if (loopList.Contains(configSetting)) {
				throw new ConfigException("配置节循环继承 " + configSetting.Name);
			}
			if(doneList.Contains(configSetting)) {
				return;
			}
			loopList.Add(configSetting);
			if (recursive) {
				if(configSetting.Children > 0) {
					foreach(var setting in configSetting.GetChildSettings()) {
						ConfigSettingInherit(setting, loopList, doneList);
					}
				}
			}
			if(inherit) {
				if(!string.IsNullOrEmpty(inheritPath)) {
					var inheritSetting = configSetting.GetChildSetting(inheritPath);
					if(inheritSetting != null) {
						ConfigSettingInherit(inheritSetting, loopList, doneList);
						configSetting.CopyFrom(inheritSetting);
					}
				}
			}
			loopList.Remove(configSetting);
			doneList.Add(configSetting);
		}
	}
}

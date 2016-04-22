/* Copyright(c) 2005-2011 R2@DevFx.NET, License(LGPL) */

using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using HTB.DevFx.Core;
using HTB.DevFx.Utils;
using HTB.DevFx.Core.Config;

namespace HTB.DevFx.Config
{
	public abstract class ConfigServiceBase : ServiceBase, IConfigService, IConfigServiceInternal
	{
		protected virtual void Init(IStartupSetting startupSetting) {
			ThreadHelper.ThreadSafeExecute(this, () => this.Settings == null, () => this.OnInit(startupSetting));
		}

		protected virtual void OnInit(IStartupSetting startupSetting) {
			var appSetting = ConfigSectionHandler.GetConfig("htb.devfx");
			var setting = startupSetting ?? appSetting.ToSetting<StartupSetting>("startup");
			var assemblies = new List<Assembly>();
			if (setting != null && setting.CoreSetting != null && setting.CoreSetting.ConfigServiceSetting != null && setting.CoreSetting.ConfigServiceSetting.Assemblies != null) {
				var assemblySettings = setting.CoreSetting.ConfigServiceSetting.Assemblies;
				foreach (var assembly in assemblySettings) {
					var dll = TypeHelper.AssemblyLoad(assembly.AssemblyName, false);
					if (dll != null && !assemblies.Contains(dll)) {
						assemblies.Add(dll);
					}
				}
			}

			var rootSetting = ConfigHelper.LoadConfigSettingFromAssemblies(new[] { typeof(Esb.IServiceLocator).Assembly, typeof(ConfigService).Assembly }, assemblies, appSetting, setting);
			setting = rootSetting.ToSetting<StartupSetting>("htb.devfx/startup");
			IConfigServiceSetting configServiceSetting = null;
			if(setting != null && setting.CoreSetting != null && setting.CoreSetting.ConfigServiceSetting != null) {
				configServiceSetting = setting.CoreSetting.ConfigServiceSetting;
				if(setting.CoreSetting.ConfigServiceSetting.ConfigFiles != null && setting.CoreSetting.ConfigServiceSetting.ConfigFiles.Length > 0) {
					var configFiles = new List<string>();
					foreach(var fileSetting in setting.CoreSetting.ConfigServiceSetting.ConfigFiles) {
						var files = FileHelper.SearchFileWithPattern(fileSetting.FileName, SearchOption.AllDirectories, AppDomain.CurrentDomain.SetupInformation.ApplicationBase);
						if (files != null && files.Length > 0) {
							configFiles.AddRange(files);
						}
					}
					foreach (var fileName in configFiles) {
						var configSetting = ConfigHelper.CreateFromXmlFile(fileName);
						rootSetting.Merge(configSetting);
					}
				}
			}

			ConfigSettingInherit(rootSetting["htb.devfx"], new List<IConfigSetting>(), new List<IConfigSetting>());
			this.Settings = rootSetting;
			if(configServiceSetting != null && configServiceSetting.Debug != null && configServiceSetting.Debug.Enabled) {
				var outputFileName = configServiceSetting.Debug.FileName;
				if(!string.IsNullOrEmpty(outputFileName)) {
					outputFileName = FileHelper.GetPhysicalPath(".", outputFileName, true);
					File.WriteAllText(outputFileName, rootSetting.ToString());
				}
			}
		}

		private sealed class ConfigInheritInternal
		{
			public string Base { get; set; }
			public bool Recursive { get; set; }
		}
		private const string ConfigInheritPropertyName = "configInherit";
		private static void ConfigSettingInherit(IConfigSetting configSetting, List<IConfigSetting> loopList, List<IConfigSetting> doneList) {
			if(configSetting == null) {
				return;
			}
			var inheritPath = configSetting.Property.TryGetPropertyValue(ConfigInheritPropertyName);
			if(string.IsNullOrEmpty(inheritPath)) {
				return;
			}
			var recursive = false;
			var inherit = false;
			if(!inheritPath.StartsWith("..")) {
				if(!bool.TryParse(inheritPath, out recursive)) {
					if(inheritPath.StartsWith("{") && inheritPath.EndsWith("}")) {
						var configInherit = JsonHelper.FromJson<ConfigInheritInternal>(inheritPath, false);
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

		protected virtual IConfigSetting Settings { get; private set; }

		protected virtual IConfigSetting GetSetting(Type type) {
			if (typeof(IObjectService).IsAssignableFrom(type)) {
				return this.Settings.GetChildSetting("htb.devfx/startup");
			}
			return this.ObjectService.GetObjectSetting(type);
		}

		#region IConfigServiceInternal Members

		void IInitializable<IStartupSetting>.Init(IStartupSetting startupSetting) {
			this.Init(startupSetting);
		}

		#endregion

		#region IConfigService Members

		IConfigSetting IConfigService.Settings {
			get { return this.Settings; }
		}

		IConfigSetting IConfigService.GetSetting<T>() {
			return this.GetSetting(typeof(T));
		}

		IConfigSetting IConfigService.GetSetting(Type type) {
			return this.GetSetting(type);
		}

		IConfigSetting IConfigService.GetSetting(object target) {
			return target != null ? this.GetSetting(target.GetType()) : null;
		}

		IConfigSetting IConfigService.GetSetting(string xpath) {
			return this.Settings.GetChildSetting(xpath);
		}

		#endregion
	}
}

/* Copyright(c) 2005-2011 R2@DevFx.NET, License(LGPL) */

namespace HTB.DevFx.Config
{
	internal class ConfigServiceSetting : ConfigSettingElement, IConfigServiceSetting
	{
		protected override void OnConfigSettingChanged() {
			base.OnConfigSettingChanged();
			this.TypeName = this.GetSetting("type");
			this.Debug = this.GetSetting<ConfigDebugSetting>("debug");
			this.Assemblies = this.GetSettings<ConfigAssemblySetting>("assemblies", null).ToArray();
			this.ConfigFiles = this.GetSettings<ConfigFileSetting>("configFiles", null).ToArray();
		}

		public string TypeName { get; private set; }
		public IConfigDebugSetting Debug { get; private set; }
		public IConfigAssemblySetting[] Assemblies { get; private set; }
		public IConfigFileSetting[] ConfigFiles { get; private set; }

		internal class ConfigAssemblySetting : ConfigSettingElement, IConfigAssemblySetting
		{
			protected override void OnConfigSettingChanged() {
				base.OnConfigSettingChanged();
				this.AssemblyName = this.GetSetting("assembly");
			}

			public string AssemblyName { get; private set; }
		}

		internal class ConfigFileSetting : ConfigSettingElement, IConfigFileSetting
		{
			protected override void OnConfigSettingChanged() {
				base.OnConfigSettingChanged();
				this.FileName = this.GetSetting("file");
			}

			public string FileName { get; private set; }
		}

		internal class ConfigDebugSetting: ConfigSettingElement, IConfigDebugSetting
		{
			protected override void OnConfigSettingChanged() {
				base.OnConfigSettingChanged();
				this.Enabled = this.GetSetting("enabled", false);
				this.FileName = this.GetSetting("outputFile");
			}

			public bool Enabled { get; private set; }
			public string FileName { get; private set; }
		}
	}
}

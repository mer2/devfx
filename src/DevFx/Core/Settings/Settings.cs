using DevFx.Configuration;
[assembly: ConfigResource("res://DevFx.Core.Settings.Objects.config", Index = -100)]

namespace DevFx.Core.Settings
{
	/*
	<configuration>
		<devfx>
			<startup>
				<assemblies>
					<add name="" />
				</assemblies>
				<debug enabled="false" file="" />
				<configFiles>
					<add name="" />
				</configFiles>
			</startup>
			<container configSet="{tag:'object'}">
				<typeAliases>
					<add name="" type="" />
				</typeAliases>
				<constAliases>
					<add name="" type="" value="" />
				</constAliases>
				<object name="" type="" lifetime="" builder="">
					<constructor configSet="{tag:'parameter'}">
						<parameter name="" type="" value="" />
					</constructor>
					<properties configSet="{tag:'property'}">
						<property name="" type="" value="" />
					</properties>
					<dependencies>
						<add name="" />
					</dependencies>
				</object>
			</container>
		</devfx>
	</configuration>
	*/
	internal class ObjectServiceSetting : ConfigSettingElement, IObjectServiceSetting
	{
		protected override void OnConfigSettingChanged() {
			base.OnConfigSettingChanged();
			this.Container = this.GetSetting<ObjectNamespaceSetting>("container");
		}

		public IObjectNamespaceSetting Container { get; private set; }

		internal class ObjectSetting : TypeSetting, IObjectSetting
		{
			protected override void OnConfigSettingChanged() {
				base.OnConfigSettingChanged();

				this.ConstructorParameters = this.GetSettings<ValueSetting>("constructor", null).ToArray();
				this.Dependencies = this.GetSettings<NameSetting>("dependencies", null).ToArray();
				this.Properties = this.GetSettings<ValueSetting>("properties", null).ToArray();
				this.Lifetime = this.GetSetting<string>("lifetime");
				this.Builder = this.GetSetting<string>("builder");
			}

			public IValueSetting[] ConstructorParameters { get; private set; }
			public INameSetting[] Dependencies { get; private set; }
			public IValueSetting[] Properties { get; private set; }
			public string Lifetime { get; private set; }
			public string Builder { get; private set; }
		}

		internal class ObjectNamespaceSetting : ConfigSettingElement, IObjectNamespaceSetting
		{
			protected override void OnConfigSettingChanged() {
				base.OnConfigSettingChanged();
				this.TypeAliases = this.GetSettings<TypeSetting>("typeAliases", null).ToArray();
				this.ConstAliases = this.GetSettings<ValueSetting>("constAliases", null).ToArray();
				this.Objects = this.GetSettings<ObjectSetting>("object").ToArray();
			}

			public ITypeSetting[] TypeAliases { get; private set; }
			public IValueSetting[] ConstAliases { get; private set; }
			public IObjectSetting[] Objects { get; private set; }
		}
	}

	internal class StartupSetting : ConfigSettingElement
	{
		protected override void OnConfigSettingChanged() {
			base.OnConfigSettingChanged();
			this.Debug = this.GetSetting<ConfigDebugSetting>("debug");
			this.Assemblies = this.GetSettings<NameSetting>("assemblies", null).ToArray();
			this.ResourceFiles = this.GetSettings<NameSetting>("resourceFiles", null).ToArray();
			this.ConfigFiles = this.GetSettings<NameSetting>("configFiles", null).ToArray();
		}

		public ConfigDebugSetting Debug { get; private set; }
		public NameSetting[] Assemblies { get; private set; }
		public NameSetting[] ResourceFiles { get; private set; }
		public NameSetting[] ConfigFiles { get; private set; }

		internal class ConfigDebugSetting : ConfigSettingElement
		{
			protected override void OnConfigSettingChanged() {
				base.OnConfigSettingChanged();
				this.Enabled = this.GetSetting("enabled", false);
				this.FileName = this.GetSetting("file", "devfx.debugconfig");
			}

			public bool Enabled { get; private set; }
			public string FileName { get; private set; }
		}
	}
}
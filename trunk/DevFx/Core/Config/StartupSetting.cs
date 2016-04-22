/* Copyright(c) 2005-2011 R2@DevFx.NET, License(LGPL) */

using HTB.DevFx.Config;
using HTB.DevFx.Utils;
[assembly: ConfigResource("res://HTB.DevFx.Core.Config.htb.devfx.core.config", Index = 0)]
[assembly: ConfigResource("res://HTB.DevFx.Core.Config.htb.devfx.objects.config", Index = 10)]

namespace HTB.DevFx.Core.Config
{
	internal class StartupSetting : ConfigSettingElement, IStartupSetting
	{
		protected override void OnConfigSettingChanged() {
			base.OnConfigSettingChanged();
			this.coreSetting = this.GetSetting<CoreSetting>("core");
		}

		private ICoreSetting coreSetting;
		ICoreSetting IStartupSetting.CoreSetting {
			get { return this.coreSetting; }
		}

		internal class CoreSetting : ConfigSettingElement, ICoreSetting
		{
			protected override void OnConfigSettingChanged() {
				base.OnConfigSettingChanged();
				this.ConfigServiceSetting = this.GetSetting<ConfigServiceSetting>("configService");
				this.objectServiceSetting = this.GetSetting<ObjectServiceSetting>("objectService");
			}

			public IConfigServiceSetting ConfigServiceSetting { get; private set; }

			private IObjectServiceSetting objectServiceSetting;
			IObjectServiceSetting ICoreSetting.ObjectServiceSetting {
				get { return this.objectServiceSetting; }
			}
		}

		internal class ObjectServiceSetting : ConfigSettingElement, IObjectServiceSetting
		{
			protected override void OnConfigSettingChanged() {
				base.OnConfigSettingChanged();
				this.TypeName = this.GetSetting("type");
				this.Extenders = this.GetSettings<ObjectServiceExtenderSetting>("extenders", null).ToArray();
				this.ObjectNamespace = this.ConfigSetting.GetChildSetting("../../..").GetSetting<ObjectNamespaceSetting>("objects");
			}

			public string TypeName { get; private set; }
			public IObjectServiceExtenderSetting[] Extenders { get; private set; }
			public IObjectNamespaceSetting ObjectNamespace { get; private set; }
		}

		internal class ObjectServiceExtenderSetting : ConfigSettingElement, IObjectServiceExtenderSetting
		{
			protected override void OnConfigSettingChanged() {
				base.OnConfigSettingChanged();
				this.TypeName = this.GetSetting("type");
				this.Enabled = this.GetSetting("enabled", true);
			}

			public string TypeName { get; private set; }
			public bool Enabled { get; private set; }
		}

		internal class ObjectNamespaceSetting : ConfigSettingElement, IObjectNamespaceSetting
		{
			protected override void OnConfigSettingChanged() {
				base.OnConfigSettingChanged();
				this.Name = this.GetSetting("name") ?? ObjectServiceBase.GlobalObjectNamespaceName;
				this.TypeAliases = this.GetSettings<TypeAliasSetting>("typeAliases", null).ToArray();
				this.ConstAliases = this.GetSettings<ValueSetting>("constAliases", null).ToArray();
				this.ObjectSettings = this.GetSettings<ObjectSetting>("object").ToArray();
				this.ObjectNamespaces = this.GetSettings<ObjectNamespaceSetting>("namespace", "objects").ToArray();
			}

			public string Name { get; private set; }
			public ITypeSetting[] TypeAliases { get; private set; }
			public IValueSetting[] ConstAliases { get; private set; }
			public IObjectSetting[] ObjectSettings { get; private set; }
			public IObjectNamespaceSetting[] ObjectNamespaces { get; private set; }
		}

		internal class ObjectSetting : Esb.Config.ObjectSetting, IObjectSetting
		{
			protected override void OnConfigSettingChanged() {
				base.OnConfigSettingChanged();
				this.Namespace = this.ConfigSetting.Parent.GetSetting("name") ?? string.Empty;
				this.GroupName = this.GetSetting("group");
				this.Constructor = this.GetSetting<ConstructorSetting>("constructor");
				this.Dependencies = this.GetSettings<DependencySetting>("dependencies", null).ToArray();
				this.Properties = this.GetSettings<ValueSetting>("properties", null).ToArray();
				this.Lifetime = this.GetSetting<LifetimeSetting>("lifetime");
			}

			public string Namespace { get; private set; }
			public string GroupName { get; private set; }

			public IConstructorSetting Constructor { get; private set; }
			public IDependencySetting[] Dependencies { get; private set; }
			public IValueSetting[] Properties { get; private set; }
			public ILifetimeSetting Lifetime { get; private set; }

			internal class ConstructorSetting : ConfigSettingElement, IConstructorSetting
			{
				protected override void OnConfigSettingChanged() {
					base.OnConfigSettingChanged();
					this.Parameters = this.GetSettings<ValueSetting>("parameter").ToArray();
				}

				public IValueSetting[] Parameters { get; private set; }
			}

			internal class DependencySetting : ConfigSettingElement, IDependencySetting
			{
				protected override void OnConfigSettingChanged() {
					base.OnConfigSettingChanged();
					this.Name = this.GetSetting("name");
				}

				public string Name { get; private set; }
			}

			internal class LifetimeSetting : ConfigSettingElement, ILifetimeSetting
			{
				protected override void OnConfigSettingChanged() {
					base.OnConfigSettingChanged();
					this.TypeName = this.GetSetting("type");
				}

				public string TypeName { get; private set; }
			}

			internal class SettingSetInternal : IValueSetting
			{
				public string Name { get; set; }
				public string Type { get; set; }
				public string Value { get; set; }

				public static SettingSetInternal Create(string settingSet) {
					SettingSetInternal instance = null;
					if (!string.IsNullOrEmpty(settingSet)) {
						instance = JsonHelper.FromJson<SettingSetInternal>(settingSet, false);
					}
					return instance;
				}

				#region Implementation of ITypeSetting

				string ITypeSetting.TypeName {
					get { return this.Type; }
				}

				#endregion
			}
		}

		internal class TypeAliasSetting : ConfigSettingElement, ITypeSetting
		{
			protected override void OnConfigSettingChanged() {
				base.OnConfigSettingChanged();
				this.Name = this.GetSetting("name");
				this.TypeName = this.GetSetting("type");
			}

			public string Name { get; private set; }
			public string TypeName { get; private set; }
		}

		internal class ValueSetting : TypeAliasSetting, IValueSetting
		{
			protected override void OnConfigSettingChanged() {
				base.OnConfigSettingChanged();
				var value = this.GetSetting("value");
				if(value == null) {
					value = this.ConfigSetting.Value.Value;
				}
				this.Value = value;
			}

			public string Value { get; private set; }
		}
	}
}

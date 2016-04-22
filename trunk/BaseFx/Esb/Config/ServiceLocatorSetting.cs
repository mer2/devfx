/* Copyright(c) 2005-2011 R2@DevFx.NET, License(LGPL) */

using HTB.DevFx.Config;
using HTB.DevFx.Core.Config;

[assembly: ConfigResource("res://HTB.DevFx.Esb.Config.htb.devfx.servicelocator.config", Index = 0)]

namespace HTB.DevFx.Esb.Config
{
	internal class StartupSetting : ConfigSettingElement
	{
		protected override void OnConfigSettingChanged() {
			base.OnConfigSettingChanged();
			this.CoreSetting = this.GetTypedSetting<CoreSetting>("core");
		}

		public CoreSetting CoreSetting { get; private set; }
	}

	internal class CoreSetting : ConfigSettingElement
	{
		protected override void OnConfigSettingChanged() {
			base.OnConfigSettingChanged();
			this.ServiceLocatorSetting = this.GetTypedSetting<ServiceLocatorSetting>("serviceLocator");
		}

		public ServiceLocatorSetting ServiceLocatorSetting { get; private set; }
	}

	internal class ServiceLocatorSetting : ConfigSettingElement, IServiceLocatorSetting
	{
		protected override void OnConfigSettingChanged() {
			base.OnConfigSettingChanged();
			this.TypeName = this.GetSetting("type");
			this.Extenders = this.GetSettings<ServiceLocatorExtenderSetting>("extenders", null).ToArray();
			this.ContainerPath = this.GetSetting("containerPath", "../../../objects");
			this.Container = DevFx.Config.ConfigSetting.ToSetting<ObjectContainerSetting>(this.ConfigSetting, this.ContainerPath);
		}

		public string TypeName { get; private set; }
		public ITypeSetting[] Extenders { get; private set; }
		public string ContainerPath { get; private set; }
		public ObjectContainerSetting Container { get; private set; }
	}

	internal class ObjectContainerSetting : ConfigSettingElement
	{
		protected override void OnConfigSettingChanged() {
			base.OnConfigSettingChanged();
			this.TypeAliases = this.GetSettings<TypeSetting>("typeAliases", null).ToArray();
			this.Objects = this.GetSettings<ObjectSetting>("object").ToArray();
		}

		public ITypeSetting[] TypeAliases { get; private set; }
		public IObjectSettingInternal[] Objects { get; private set; }
	}

	internal class ServiceLocatorExtenderSetting : ConfigSettingElement, ITypeSetting
	{
		protected override void OnConfigSettingChanged() {
			base.OnConfigSettingChanged();
			this.Name = this.GetSetting("name");
			this.TypeName = this.GetSetting("type");
		}

		public string Name { get; private set; }
		public string TypeName { get; private set; }
	}
}
/* Copyright(c) 2005-2011 R2@DevFx.NET, License(LGPL) */

using HTB.DevFx.Config;

namespace HTB.DevFx.Remoting.Config
{
	internal class RemotingServiceSetting : ConfigSettingElement
	{
		protected override void OnConfigSettingChanged() {
			base.OnConfigSettingChanged();
			this.ConfigFile = this.GetSetting("configFile");
			this.Services = this.GetSettings<RemotingServiceItemSetting>(null).ToArray();
		}

		public string ConfigFile { get; private set; }
		public RemotingServiceItemSetting[] Services { get; private set; }
	}

	internal class RemotingServiceItemSetting : ConfigSettingElement
	{
		protected override void OnConfigSettingChanged() {
			base.OnConfigSettingChanged();
			this.Name = this.GetSetting("name");
			this.TypeName = this.GetSetting("type");
			this.ServiceTypeName = this.GetSetting("serviceType");
		}
		
		public string Name { get; private set; }
		public string TypeName { get; private set; }
		public string ServiceTypeName { get; private set; }
	}
}

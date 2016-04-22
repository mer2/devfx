/* Copyright(c) 2005-2011 R2@DevFx.NET, License(LGPL) */

using HTB.DevFx.Config;

[assembly: ConfigResource("res://HTB.DevFx.Web.Config.htb.devfx.web.config", Index = 400)]

namespace HTB.DevFx.Web.Config
{
	internal class HttpModuleContextSetting : ConfigSettingElement, IHttpModuleContextSetting
	{
		protected override void OnConfigSettingChanged() {
			this.HttpApplications = this.GetSettings<HttpModuleSetting>(null).ToArray();
		}

		public IHttpModuleSetting[] HttpApplications { get; private set; }
	}

	internal class HttpModuleSetting : ConfigSettingElement, IHttpModuleSetting
	{
		protected override void OnConfigSettingChanged() {
			this.Name = this.GetSetting("name");
			this.TypeName = this.GetSetting("type");
		}

		public string Name { get; private set; }
		public string TypeName { get; private set; }
	}
}

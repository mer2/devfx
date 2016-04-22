/* Copyright(c) 2005-2011 R2@DevFx.NET, License(LGPL) */

using HTB.DevFx.Config;

namespace HTB.DevFx.ServiceRunners.Config
{
	internal class ServiceRunnerHostSetting : ConfigSettingElement
	{
		protected override void OnConfigSettingChanged() {
			base.OnConfigSettingChanged();
			this.RunAs = this.GetSetting("runAs");
			this.Title = this.GetSetting("title", "服务运行器");
		}
		public string RunAs { get; private set; }
		public string Title { get; private set; }
	}
}

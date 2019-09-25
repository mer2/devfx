using DevFx.Configuration;

namespace DevFx.Esb.Server.Settings
{
	[SettingObject("~/esb/server")]
	internal class ServiceFactorySetting : ConfigSettingElement
	{
		protected override void OnConfigSettingChanged() {
			base.OnConfigSettingChanged();
			this.Debug = this.GetSetting("debug", false);
			this.RouteUrl = this.GetSetting("routeUrl", "/Services/");
			this.PathRegex = this.GetSetting<string>("pathRegex", "(?<serviceName>\\w+)(/(?<methodName>\\w+))?");
		}

		public bool Debug { get; private set; }
		public string RouteUrl { get; private set; }
		public string PathRegex { get; private set; }
	}
}

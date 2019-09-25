using DevFx.Configuration;

namespace DevFx.Esb.Client.Settings
{
	[SettingObject("~/esb/client")]
	internal class HttpRealProxyFactorySetting : ConfigSettingElement
	{
		protected override void OnConfigSettingChanged() {
			base.OnConfigSettingChanged();
			this.UrlMapping = this.GetSettings<ServiceUrlSetting>("urlMapping", null).ToArray();
		}

		public ServiceUrlSetting[] UrlMapping { get; private set; }
	}

	//服务Url或别名的本地映射，比如esb://octopus.smsservice映射成http://localhost/Services/SmsService，一般用于调试
	internal class ServiceUrlSetting : ConfigSettingElement
	{
		protected override void OnConfigSettingChanged() {
			base.OnConfigSettingChanged();
			this.Name = this.GetSetting<string>("name", required: true);
			this.Url = this.GetSetting<string>("url", required: true);
		}

		public string Name { get; private set; }//服务别名
		public string Url { get; private set; }//对应的服务地址
	}
}

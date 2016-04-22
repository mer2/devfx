/* Copyright(c) 2005-2011 R2@DevFx.NET, License(LGPL) */

using HTB.DevFx.Utils;

namespace HTB.DevFx.Config.XmlConfig
{
	partial class XmlConfigSetting
	{
		 static partial void CheckConfigProvider(ConfigSetting setting, ref bool sourceDone) {
			var configProvider = setting.ConfigProvider;
			if (!string.IsNullOrEmpty(configProvider)) {
				var providerType = TypeHelper.CreateType(configProvider, false);
				if (providerType != null) {
					object returnValue;
					if (TypeHelper.TryInvoke(providerType, "GetConfigSetting", out returnValue, false, setting.ConfigSource)) {
						var configSetting = returnValue as ConfigSetting;
						if (configSetting != null) {
							setting.Merge(configSetting);
							sourceDone = true;
						}
					}
				}
			}
		}
	}
}

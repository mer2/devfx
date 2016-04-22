/* Copyright(c) 2005-2011 R2@DevFx.NET, License(LGPL) */

namespace HTB.DevFx.Config
{
	public static class ConfigServiceExtensions
	{
		public static TSetting ToSetting<TObject, TSetting>(this IConfigService configService) where TSetting : IConfigSettingElement, new() {
			if (configService != null) {
				var config = configService.GetSetting<TObject>();
				if(config != null) {
					return config.ToSetting<TSetting>();
				}
			}
			return default(TSetting);
		}
	}
}

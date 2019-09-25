/* Copyright(c) 2005-2011 R2@DevFx.NET, License(LGPL) */

using DevFx.Configuration;

namespace DevFx.Exceptions.Settings
{
	[SettingObject("~/exception")]
	internal class ExceptionServiceSetting : ConfigSettingElement, IExceptionServiceSetting
	{
		protected override void OnConfigSettingChanged() {
			this.Enabled = this.GetSetting("enabled", true);
		}

		public bool Enabled { get; private set; }
	}
}
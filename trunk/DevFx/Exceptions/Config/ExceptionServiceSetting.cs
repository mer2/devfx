/* Copyright(c) 2005-2011 R2@DevFx.NET, License(LGPL) */

using HTB.DevFx.Config;

[assembly: ConfigResource("res://HTB.DevFx.Exceptions.Config.htb.devfx.exceptions.config", Index = 600)]

namespace HTB.DevFx.Exceptions.Config
{
	internal class ExceptionServiceSetting : ConfigSettingElement, IExceptionServiceSetting
	{
		protected override void OnConfigSettingChanged() {
			this.Enabled = this.GetSetting("enabled", false);
			this.ExceptionHandlers = this.GetSettings<ExceptionHandlerSetting>("handlers", null).ToArray();
		}

		public bool Enabled { get; private set; }
		public IExceptionHandlerSetting[] ExceptionHandlers { get; private set; }

		internal class ExceptionHandlerSetting : ConfigSettingElement, IExceptionHandlerSetting
		{
			protected override void OnConfigSettingChanged() {
				this.Enabled = this.GetSetting("enabled", false);
				this.HandlerTypeName = this.GetSetting("type");
			}

			public bool Enabled { get; private set; }
			public string HandlerTypeName { get; private set; }
		}
	}
}

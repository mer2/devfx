/* Copyright(c) 2005-2011 R2@DevFx.NET, License(LGPL) */

using HTB.DevFx.Config;
using HTB.DevFx.Core.Config;
using HTB.DevFx.Esb.Config;

[assembly: ConfigResource("res://HTB.DevFx.Remoting.Config.htb.devfx.remoting.config", Index = 1)]

namespace HTB.DevFx.Remoting.Config
{
	internal class RemotingObjectBuilderFactorySetting : ConfigSettingElement
	{
		protected override void OnConfigSettingChanged() {
			base.OnConfigSettingChanged();
			this.HandlerContext = this.GetTypedSetting<HandlerContext>("handlers");
		}

		public HandlerContext HandlerContext { get; private set; }
	}

	internal class HandlerContext : ConfigSettingElement
	{
		protected override void OnConfigSettingChanged() {
			base.OnConfigSettingChanged();
			this.DefaultHandler = this.GetSetting("defaultHandler");
			this.Handlers = this.GetSettings<TypeSetting>(null).ToArray();
		}

		public string DefaultHandler { get; private set; }
		public ITypeSetting[] Handlers { get; private set; }
	}
}

/* Copyright(c) 2005-2011 R2@DevFx.NET, License(LGPL) */

using HTB.DevFx.Config;

[assembly: ConfigResource("res://HTB.DevFx.Mail.Config.htb.devfx.mail.config", Index = 300)]

namespace HTB.DevFx.Mail.Config
{
	internal class MailServiceSetting : ConfigSettingElement,  IMailServiceSetting
	{
		protected override void OnConfigSettingChanged() {
			this.SmtpServer = this.GetSetting("smtpServer");
			this.ServerPort = this.GetSetting("serverPort", 25);
			this.UserName = this.GetSetting("userName");
			this.Password = this.GetSetting("password");
			this.Interval = this.GetSetting("interval", 1000D);
		}

		public string SmtpServer { get; private set; }
		public int ServerPort { get; private set; }
		public string UserName { get; private set; }
		public string Password { get; private set; }
		public double Interval { get; private set; }
	}
}

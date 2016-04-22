/* Copyright(c) 2005-2011 R2@DevFx.NET, License(LGPL) */

namespace HTB.DevFx.Mail.Config
{
	/// <summary>
	/// 邮件发送工具配置
	/// </summary>
	public interface IMailServiceSetting
	{
		/// <summary>
		/// SMTP服务器地址
		/// </summary>
		string SmtpServer { get; }

		/// <summary>
		/// SMTP服务器侦听端口
		/// </summary>

		int ServerPort { get; }
		/// <summary>
		/// 认证用户名
		/// </summary>
		string UserName { get; }

		/// <summary>
		/// 认证用户密码
		/// </summary>
		string Password { get; }

		/// <summary>
		/// 缓存发送时定时器间隔时间（毫秒）
		/// </summary>
		double Interval { get; }
	}
}

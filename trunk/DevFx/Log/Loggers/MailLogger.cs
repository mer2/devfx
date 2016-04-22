/* Copyright(c) 2005-2011 R2@DevFx.NET, License(LGPL) */

using HTB.DevFx.Mail;

namespace HTB.DevFx.Log.Loggers
{
	/// <summary>
	/// 以邮件发送方式的日志记录器
	/// </summary>
	public class MailLogger : LoggerBase
	{
		/// <summary>
		/// 构造函数
		/// </summary>
		/// <param name="mailService">邮件服务实例</param>
		public MailLogger(IMailService mailService) {
			this.MailService = mailService;
		}

		/// <summary>
		/// 邮件服务实例
		/// </summary>
		protected IMailService MailService { get; set; }

		/// <summary>
		/// 邮件发送者
		/// </summary>
		protected string MailFrom { get; set; }

		/// <summary>
		/// 邮件接收者
		/// </summary>
		protected string MailTo { get; set; }

		/// <summary>
		/// 邮件主题
		/// </summary>
		protected string MailSubject { get; set; }

		#region Overrides of LoggerBase

		/// <summary>
		/// 实际的日志写入处理过程
		/// </summary>
		/// <param name="sender">调用者</param>
		/// <param name="args"><see cref="LogEventArgs"/> 数组</param>
		protected override void WriteLogInternal(object sender, LogEventArgs[] args) {
			if (this.MailService != null) {
				var msg = this.LogFormat(sender, args, false);
				this.MailService.Send(this.MailFrom, this.MailTo, this.MailSubject, msg);
			}
		}

		#endregion
	}
}
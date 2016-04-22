/* Copyright(c) 2005-2011 R2@DevFx.NET, License(LGPL) */

using System.Net.Mail;

namespace HTB.DevFx.Mail
{
	/// <summary>
	/// 邮件发送服务接口
	/// </summary>
	public interface IMailService
	{
		/// <summary>
		/// 发送邮件
		/// </summary>
		/// <param name="message">MailMessage实体</param>
		void Send(MailMessage message);

		/// <summary>
		/// 发送邮件
		/// </summary>
		/// <param name="message">MailMessage实体</param>
		/// <param name="queued">是否缓存邮件（提高程序响应速度）</param>
		void Send(MailMessage message, bool queued);

		/// <summary>
		/// 发送邮件
		/// </summary>
		/// <param name="from">发送者地址</param>
		/// <param name="to">接收者地址（可填多个地址，用英文分号“;”分割）</param>
		/// <param name="subject">邮件主题</param>
		/// <param name="messageText">邮件内容</param>
		void Send(string from, string to, string subject, string messageText);

		/// <summary>
		/// 发送邮件
		/// </summary>
		/// <param name="from">发送者地址</param>
		/// <param name="to">接收者地址（可填多个地址，用英文分号“;”分割）</param>
		/// <param name="subject">邮件主题</param>
		/// <param name="messageText">邮件内容</param>
		/// <param name="queued">是否缓存邮件（提高程序响应速度）</param>
		void Send(string from, string to, string subject, string messageText, bool queued);
	}
}

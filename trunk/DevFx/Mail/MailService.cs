/* Copyright(c) 2005-2011 R2@DevFx.NET, License(LGPL) */

using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Mail;
using HTB.DevFx.Core;
using HTB.DevFx.Log;
using HTB.DevFx.Mail.Config;
using HTB.DevFx.Utils;

namespace HTB.DevFx.Mail
{
	public class MailService : ServiceBase<IMailServiceSetting>, IMailService
	{
		protected class MailServiceInternal : TimerBase
		{
			protected override void OnTimer() {
				if (this.MessageQueue != null) {
					var list = new List<MailMessage>();
					lock (this.MessageQueue) {
						while (this.MessageQueue.Count > 0) {
							list.Add(this.MessageQueue.Dequeue());
						}
					}
					var smtp = GetSmtpClient();
					foreach (var message in list) {
						try {
							smtp.Send(message);
						} catch (Exception ex) {
							if (this.LogService != null) {
								var title = message.Subject;
								var to = message.To.ToString();
								this.LogService.WriteLog(this, LogLevel.ERROR, "Mail Send Failed\r\nTitle: {0}\r\nTo: {1}\r\nException:\r\n{2}", title, to, ex);
							}
						}
					}
				}
			}

			protected override double Interval {
				get { return this.Setting.Interval; }
			}

			/// <summary>
			/// 待发邮件队列
			/// </summary>
			protected Queue<MailMessage> MessageQueue { get; set; }

			/// <summary>
			/// 日志服务
			/// </summary>
			protected internal ILogService LogService { get; set; }

			protected internal IMailServiceSetting Setting { get; set; }

			/// <summary>
			/// 发送邮件
			/// </summary>
			/// <param name="message">MailMessage实体</param>
			/// <param name="queued">是否缓存邮件（提高程序响应速度）</param>
			protected internal virtual void SendInternal(MailMessage message, bool queued) {
				if (queued) {
					if (this.MessageQueue == null) {
						lock (this) {
							if (this.MessageQueue == null) {
								this.MessageQueue = new Queue<MailMessage>();
							}
						}
					}
					lock (this.MessageQueue) {
						this.MessageQueue.Enqueue(message);
					}
					this.StartTimer();
				} else {
					GetSmtpClient().Send(message);
				}
			}

			protected virtual SmtpClient GetSmtpClient() {
				var server = this.Setting.SmtpServer;
				var port = this.Setting.ServerPort;
				var userName = this.Setting.UserName;
				var password = this.Setting.Password;
				var smtp = string.IsNullOrEmpty(server) ? new SmtpClient() : new SmtpClient(server, port);
				if (!string.IsNullOrEmpty(userName) && !string.IsNullOrEmpty(password)) {
					smtp.Credentials = new NetworkCredential(userName, password);
				} else if(!string.IsNullOrEmpty(server)) {
					smtp.Credentials = CredentialCache.DefaultNetworkCredentials;
				}
				return smtp;
			}
		}

		protected MailService(ILogService logService) {
			this.MailServiceInstance = new MailServiceInternal { LogService = logService };
		}

		protected MailServiceInternal MailServiceInstance { get; set; }

		protected override IMailServiceSetting Setting {
			get { return base.Setting; }
			set { this.MailServiceInstance.Setting = base.Setting = value; }
		}

		#region IMailService Members

		void IMailService.Send(MailMessage message) {
			this.MailServiceInstance.SendInternal(message, true);
		}

		void IMailService.Send(MailMessage message, bool queued) {
			this.MailServiceInstance.SendInternal(message, queued);
		}

		void IMailService.Send(string from, string to, string subject, string messageText) {
			((IMailService)this).Send(from, to, subject, messageText, true);
		}

		void IMailService.Send(string from, string to, string subject, string messageText, bool queued) {
			this.MailServiceInstance.SendInternal(new MailMessage(from, to, subject, messageText), queued);
		}

		#endregion

		#region IMailService Static Members

		internal static IMailService Current {
			get { return DevFx.ObjectService.GetObject<IMailService>(); }
		}

		public static void Send(MailMessage message) {
			Current.Send(message);
		}

		public static void Send(MailMessage message, bool queued) {
			Current.Send(message, queued);
		}

		public static void Send(string from, string to, string subject, string messageText) {
			Current.Send(from, to, subject, messageText);
		}

		public static void Send(string from, string to, string subject, string messageText, bool queued) {
			Current.Send(from, to, subject, messageText, queued);
		}

		#endregion
	}
}

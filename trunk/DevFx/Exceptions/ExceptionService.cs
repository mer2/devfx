/* Copyright(c) 2005-2011 R2@DevFx.NET, License(LGPL) */

using System;
using System.Linq;
using HTB.DevFx.Core;
using HTB.DevFx.Exceptions.Config;
using HTB.DevFx.Log;

namespace HTB.DevFx.Exceptions
{
	public class ExceptionService : ServiceBase<IExceptionServiceSetting>, IExceptionService
	{
		protected internal ExceptionService() {
		}

		protected IExceptionHandler[] ExceptionHandlers { get; set; }

		protected override void OnInit() {
			if (this.Setting.ExceptionHandlers != null && this.Setting.ExceptionHandlers.Length > 0) {
				this.ExceptionHandlers = this.Setting.ExceptionHandlers.Where(x => x.Enabled).Select(x => this.ObjectService.GetOrCreateObject<IExceptionHandler>(x.HandlerTypeName)).ToArray();
			}
		}

		protected virtual void PublishInternal(Exception e, int level) {
			if (!this.Setting.Enabled || e == null || this.ExceptionHandlers == null || this.ExceptionHandlers.Length <= 0) {
				return;
			}

			foreach (var handler in this.ExceptionHandlers) {
				if (!handler.ExceptionType.IsInstanceOfType(e)) {
					continue;
				}
				IAOPResult result;
				try {
					result = handler.Handle(e, level);
				} catch(Exception) {
					//处理错误的时候出错了？？简单的让下一个处理器处理
					continue;
				}
				if (result.ResultNo <= 0) {
					break;
				}
				if (result.ResultNo == 1) {
					if (result.ResultAttachObject != null) {
						e = (Exception)result.ResultAttachObject;
					}
				} else if (result.ResultNo == 2) {//重新轮询
					if (result.ResultAttachObject != null) {
						e = (Exception)result.ResultAttachObject;
					}
					this.PublishInternal(e, level);
					break;
				}
			}
		}

		#region IExceptionService Members

		void IExceptionService.Publish(Exception e) {
			this.PublishInternal(e, LogLevel.ERROR);
		}

		void IExceptionService.Publish(Exception e, int level) {
			this.PublishInternal(e, level);
		}

		#endregion

		#region static members

		internal static IExceptionService Current {
			get { return DevFx.ObjectService.GetObject<IExceptionService>(); }
		}

		public static void Publish(Exception e) {
			Current.Publish(e);
		}

		public static void Publish(Exception e, int level) {
			Current.Publish(e, level);
		}

		#endregion
	}
}

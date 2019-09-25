/* Copyright(c) 2005-2011 R2@DevFx.NET, License(LGPL) */

using System;
using DevFx.Core;
using DevFx.Exceptions.Settings;
using DevFx.Logging;

namespace DevFx.Exceptions
{
	[Object]
	public class ExceptionService : IExceptionService, IInitializable<IExceptionServiceSetting>
	{
		protected internal ExceptionService() {
		}

		[Autowired]
		protected IObjectService ObjectService { get; set; }
		protected IExceptionServiceSetting Setting { get; set; }
		protected IExceptionHandler[] ExceptionHandlers { get; set; }
		public void Init(IExceptionServiceSetting setting) {
			this.Setting = setting;
			this.ExceptionHandlers = this.ObjectService.GetObjects<IExceptionHandler>();
		}

		protected virtual void PublishInternal(Exception e, int level) {
			if (this.Setting == null || !this.Setting.Enabled || e == null || this.ExceptionHandlers == null || this.ExceptionHandlers.Length <= 0) {
				return;
			}

			foreach (var handler in this.ExceptionHandlers) {
				if (!handler.ExceptionType.IsInstanceOfType(e)) {
					continue;
				}
				int result;
				try {
					result = handler.Handle(e, level);
				} catch(Exception) {
					//处理错误的时候出错了？？简单的让下一个处理器处理
					continue;
				}
				if (result <= 0) {
					break;
				}
				if (result == 2) {//重新轮询
					this.PublishInternal(e, level);
					break;
				}
			}
		}

		#region IExceptionService Members

		void IExceptionService.Publish(Exception e, int level) {
			this.PublishInternal(e, level);
		}

		#endregion

		#region static members

		private static IExceptionService current;
		internal static IExceptionService Current => current ?? (current = DevFx.ObjectService.GetObject<IExceptionService>());

		public static void Publish(Exception e, int level = LogLevel.ERROR) {
			Current.Publish(e, level);
		}

		#endregion
	}
}

/* Copyright(c) 2005-2011 R2@DevFx.NET, License(LGPL) */

using System;
using HTB.DevFx.Esb;
using HTB.DevFx.Log;

namespace HTB.DevFx.Exceptions
{
	/// <summary>
	/// 异常辅助类
	/// </summary>
	public abstract class ExceptionHelper
	{
		/// <summary>
		/// 发布异常
		/// </summary>
		/// <param name="e">异常</param>
		/// <param name="level">记录异常的日志等级</param>
		/// <remarks>先找是否有<see cref="IExceptionService"/>的实现，有则调用相关方法；无则默认写日志信息</remarks>
		public static void Publish(Exception e, int level = LogLevel.ERROR) {
			var service = ServiceLocator.GetService<IExceptionService>();
			if(service != null) {
				service.Publish(e, level);
			} else {
				LogHelper.WriteLog(e.Message, level, "ERROR:", e);
			}
		}
	}
}

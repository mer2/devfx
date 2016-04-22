/* Copyright(c) 2005-2011 R2@DevFx.NET, License(LGPL) */

using System;

namespace HTB.DevFx.Exceptions
{
	/// <summary>
	/// 异常服务接口
	/// </summary>
	public interface IExceptionService
	{
		/// <summary>
		/// 处理异常
		/// </summary>
		/// <param name="e">异常</param>
		void Publish(Exception e);

		/// <summary>
		/// 处理异常
		/// </summary>
		/// <param name="e">异常</param>
		/// <param name="level">异常等级（决定由哪个日志记录器记录）</param>
		void Publish(Exception e, int level);
	}
}

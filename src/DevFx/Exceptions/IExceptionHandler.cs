/* Copyright(c) 2005-2011 R2@DevFx.NET, License(LGPL) */

using System;

namespace DevFx.Exceptions
{
	/// <summary>
	/// 异常处理器接口
	/// </summary>
	[Service]
	public interface IExceptionHandler
	{
		/// <summary>
		/// 此异常处理器处理的异常类型
		/// </summary>
		Type ExceptionType { get; }

		/// <summary>
		/// 进行异常处理（由异常管理器调用）
		/// </summary>
		/// <param name="e">异常</param>
		/// <param name="level">异常等级（传递给日志记录器处理）</param>
		/// <returns>处理结果，将影响下面的处理器</returns>
		/// <remarks>
		/// 异常管理器将根据返回的结果进行下一步的处理，约定：<br />
		///		返回值：
		///		<list type="bullet">
		///			<item><description>
		///				小于0：表示处理异常，管理器将立即退出异常处理
		///			</description></item>
		///			<item><description>
		///				0：处理正常
		///			</description></item>
		///			<item><description>
		///				1：已处理，需要下一个异常处理器进一步处理
		///			</description></item>
		///			<item><description>
		///				2：已处理，需要重新轮询异常处理器进行处理<br />
		///					此时异常管理器将重新进行异常处理
		///			</description></item>
		///		</list>
		/// </remarks>
		int Handle(Exception e, int level);
	}
}

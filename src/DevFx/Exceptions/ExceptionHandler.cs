/* Copyright(c) 2005-2011 R2@DevFx.NET, License(LGPL) */

using System;
using DevFx.Logging;

namespace DevFx.Exceptions
{
	/// <summary>
	/// 异常处理器接口实现
	/// </summary>
	[Object]
	public class ExceptionHandler : IExceptionHandler
	{
		/// <summary>
		/// 构造函数
		/// </summary>
		protected internal ExceptionHandler() {
		}

		protected virtual string GetFormattedString(Exception e, int level) {
			return $"{e}";
		}
		[Autowired(Required = true)]
		protected ILogService LogService { get; set; }

		#region IExceptionHandle Members

		/// <summary>
		/// 此异常处理器处理的异常类型
		/// </summary>
		public virtual Type ExceptionType {
			get { return typeof(Exception); }
		}

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
		public virtual int Handle(Exception e, int level) {
			this.LogService?.WriteLog(this.GetFormattedString(e, level), level, this);
			return 0;
		}

		#endregion
	}
}

/* Copyright(c) 2005-2011 R2@DevFx.NET, License(LGPL) */

using System;
using HTB.DevFx.Core;
using HTB.DevFx.Log;

namespace HTB.DevFx.Exceptions
{
	/// <summary>
	/// 异常处理器接口实现，建议应用程序自定义的处理器都从本类继承
	/// </summary>
	public class ExceptionHandler : IExceptionHandler
	{
		/// <summary>
		/// 构造函数
		/// </summary>
		/// <param name="logService">日志记录器</param>
		/// <param name="exceptionFormatter">信息收集格式化接口</param>
		protected internal ExceptionHandler(ILogService logService, IExceptionFormatter exceptionFormatter) {
			this.LogService = logService;
			this.exceptionFormatter = exceptionFormatter;
		}

		protected ILogService LogService { get; set; }

		protected virtual string GetFormattedString(Exception e, int level, object attachObject) {
			string message;
			try {
				message = this.ExceptionFormatter.GetFormatString(e, attachObject);
			} catch(Exception ex) {
				message = string.Format("获取错误信息格式时出错了，原始错误：{0}\r\n新错误：{1}\r\n", e, ex);
			}
			return message;
		}

		#region IExceptionHandle Members

		/// <summary>
		/// 此异常处理器处理的异常类型
		/// </summary>
		public virtual Type ExceptionType {
			get { return typeof(Exception); }
		}

		private IExceptionFormatter exceptionFormatter;
		/// <summary>
		/// 异常信息收集格式化
		/// </summary>
		public virtual IExceptionFormatter ExceptionFormatter {
			get { return this.exceptionFormatter; }
			set { this.exceptionFormatter = value; }
		}

		/// <summary>
		/// 进行异常处理（由异常管理器调用）
		/// </summary>
		/// <param name="e">异常</param>
		/// <param name="level">异常等级（传递给日志记录器处理）</param>
		/// <returns>处理结果，将影响下面的处理器</returns>
		/// <remarks>
		/// 异常管理器将根据返回的结果进行下一步的处理，约定：<br />
		///		返回的结果中，ResultNo值：
		///		<list type="bullet">
		///			<item><description>
		///				小于0：表示处理异常，管理器将立即退出异常处理
		///			</description></item>
		///			<item><description>
		///				0：处理正常
		///			</description></item>
		///			<item><description>
		///				1：已处理，需要下一个异常处理器进一步处理，<br />
		///				此时ResultAttachObject为返回的异常（可能与传入的异常是不一致的）
		///			</description></item>
		///			<item><description>
		///				2：已处理，需要重新轮询异常处理器进行处理<br />
		///					此时ResultAttachObject为返回的异常（可能与传入的异常是不一致的）<br />
		///					此时异常管理器将重新进行异常处理
		///			</description></item>
		///		</list>
		/// </remarks>
		public virtual IAOPResult Handle(Exception e, int level) {
			this.LogService.WriteLog(this, level, this.GetFormattedString(e, level, null));
			return AOPResult.Success();
		}

		#endregion
	}
}

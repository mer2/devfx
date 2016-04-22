/* Copyright(c) 2005-2011 R2@DevFx.NET, License(LGPL) */

using System;

namespace HTB.DevFx.Exceptions.Web
{
	/// <summary>
	/// WEB项目的Page页面异常
	/// </summary>
	/// <remarks>
	/// 在WEB项目中，能发现的异常都会包装成此类的实例
	/// </remarks>
	[Serializable]
	public class PageException : ExceptionBase
	{
		/// <summary>
		/// 构造函数
		/// </summary>
		public PageException() {
		}

		/// <summary>
		/// 构造函数
		/// </summary>
		/// <param name="message">异常消息</param>
		/// <param name="innerException">内部异常</param>
		public PageException(string message, Exception innerException) : base(message, innerException) {
		}

		/// <summary>
		/// 构造函数
		/// </summary>
		/// <param name="message">异常消息</param>
		public PageException(string message) : base(message) {
		}

		/// <summary>
		/// 构造函数
		/// </summary>
		/// <param name="errorNo">异常编号</param>
		/// <param name="message">异常消息</param>
		public PageException(int errorNo, string message) : base(errorNo, message) {
		}

		/// <summary>
		/// 构造函数
		/// </summary>
		/// <param name="errorNo">异常编号</param>
		/// <param name="message">异常消息</param>
		/// <param name="innerException">内部异常</param>
		public PageException(int errorNo, string message, Exception innerException) : base(errorNo, message, innerException) {
		}
	}
}

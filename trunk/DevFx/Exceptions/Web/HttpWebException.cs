/* Copyright(c) 2005-2011 R2@DevFx.NET, License(LGPL) */

using System;
using System.Web;

namespace HTB.DevFx.Exceptions.Web
{
	/// <summary>
	/// WEB应用的异常
	/// </summary>
	/// <remarks>
	/// 在WEB应用中，能发现的异常都会包装成此类的实例，此类异常将由<see cref="HTB.DevFx.Exceptions.Web.HttpWebExceptionHandler"/>处理
	/// </remarks>
	[Serializable]
	public class HttpWebException : ExceptionBase
	{
		/// <summary>
		/// 构造函数
		/// </summary>
		/// <param name="innerException">内部异常</param>
		/// <param name="httpApp">HttpApplication实例</param>
		public HttpWebException(Exception innerException, HttpApplication httpApp) : this(null, innerException, httpApp) {
		}

		/// <summary>
		/// 构造函数
		/// </summary>
		/// <param name="httpApp">HttpApplication实例</param>
		public HttpWebException(HttpApplication httpApp) : this(null, httpApp) {
		}

		/// <summary>
		/// 构造函数
		/// </summary>
		/// <param name="message">异常消息</param>
		/// <param name="innerException">内部异常</param>
		/// <param name="httpApp">HttpApplication实例</param>
		public HttpWebException(string message, Exception innerException, HttpApplication httpApp) : this(0, message, innerException, httpApp) {
		}

		/// <summary>
		/// 构造函数
		/// </summary>
		/// <param name="errorNo">异常编码</param>
		/// <param name="message">异常消息</param>
		/// <param name="innerException">内部异常</param>
		/// <param name="httpApp">HttpApplication实例</param>
		public HttpWebException(int errorNo, string message, Exception innerException, HttpApplication httpApp) : base(errorNo, message, innerException) {
			this.httpApp = httpApp;
		}

		private readonly HttpApplication httpApp;

		/// <summary>
		/// HttpApplication实例
		/// </summary>
		public HttpApplication HttpAppInstance {
			get { return this.httpApp; }
		}
	}
}

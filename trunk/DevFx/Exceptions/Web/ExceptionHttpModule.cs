/* Copyright(c) 2005-2011 R2@DevFx.NET, License(LGPL) */

using System;
using System.IO;
using System.Web;

namespace HTB.DevFx.Exceptions.Web
{
	/// <summary>
	/// 捕捉WEB应用异常的<see cref="IHttpModule"/>
	/// </summary>
	/// <remarks>
	/// 在web.config中添加如下的配置：
	///		<code>
	///			&lt;system.web&gt;
	///				&lt;httpModules&gt;
	///					......
	///					&lt;add name="ExceptionHttpModule" type="HTB.DevFx.Exceptions.Web.ExceptionHttpModule, HTB.DevFx" /&gt;
	///					......
	///				&lt;/httpModules&gt;
	///				......
	///			&lt;/system.web&gt;
	///		</code>
	/// 或者配置在框架配置文件中，形如：
	///		<code>
	///			&lt;htb.devfx&gt;
	///				&lt;httpModules configSet="{key:'type'}"&gt;
	///					&lt;add name="ExceptionHttpModule" type="HTB.DevFx.Exceptions.Web.ExceptionHttpModule, HTB.DevFx" /&gt;
	///				&lt;/httpModules&gt;
	///			&lt;/htb.devfx&gt;
	///		</code>
	/// </remarks>
	internal class ExceptionHttpModule : IHttpModule
	{
		/// <summary>
		/// WEB应用程序异常捕捉
		/// </summary>
		private static void WebOnError(object sender, EventArgs e) {
			var httpApp = (HttpApplication)sender;
			var ex = httpApp.Server.GetLastError();
			var ex0 = ExceptionBase.FindSourceException(ex);
			if(ex0 is FileNotFoundException) {
				return;
			}
			string message = null;
			if(ex != null) {
				message = ex.Message;
			}
			ExceptionService.Publish(new HttpWebException(message, ex, httpApp));
		}

		/// <summary>
		/// 初始化模块
		/// </summary>
		/// <param name="context"><see cref="HttpApplication"/> 实例</param>
		public virtual void Init(HttpApplication context) {
			context.Error += WebOnError;
		}

		public virtual void Dispose() {
		}
	}
}

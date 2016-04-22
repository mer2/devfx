/* Copyright(c) 2005-2011 R2@DevFx.NET, License(LGPL) */

using System;
using System.Linq;
using System.IO;
using System.Web;
using HTB.DevFx.Core;
using HTB.DevFx.Log;
using HTB.DevFx.Utils;

namespace HTB.DevFx.Exceptions.Web
{
	/// <summary>
	/// 处理WEB异常的处理器
	/// </summary>
	/// <remarks>
	///	<see cref="DefaultRedirectUrl"/>为异常时转向的页面地址，默认为空，可以为以下格式：
	/// ~/devfx/main/error.ashx?hc={0}&amp;amp;ec={1}&amp;amp;level={2}&amp;amp;msg={3}&amp;amp;url={4}
	/// 其中参数含义为：
	///	<list type="bullet">
	///		<item><description>hc：发生异常的页面Hash Code</description></item>
	///		<item><description>ec：异常编码</description></item>
	///		<item><description>level：异常等级</description></item>
	///		<item><description>msg：异常信息</description></item>
	///		<item><description>url：发生异常的页面地址</description></item>
	///	</list>
	/// <see cref="CheckRedirectFileExists"/>为是否检查转向的页面是否存在
	/// </remarks>
	public class HttpWebExceptionHandler : ExceptionHandler
	{
		/// <summary>
		/// 构造函数
		/// </summary>
		protected internal HttpWebExceptionHandler(ILogService logService, IExceptionFormatter exceptionFormatter) : base(logService, exceptionFormatter) {
		}

		public override Type ExceptionType {
			get { return typeof(HttpWebException); }
		}

		/// <summary>
		/// 是否开启自定义处理（显示自定义的信息）
		/// </summary>
		public bool CustomEnabled { get; set; }
		public string DefaultRedirectUrl { get; set; }
		public bool CheckRedirectFileExists { get; set; }
		/// <summary>
		/// 缺省显示的错误信息
		/// </summary>
		public string DefaultErrorMessage { get; set; }

		private string ignoreHttpCodes;
		private string customLocal;
		private string[] customLocals;

		/// <summary>
		/// 需要忽略的Http状态码，此时不记录日志
		/// </summary>
		public string IgnoreHttpCodes {
			get { return this.ignoreHttpCodes; }
			set {
				if(!string.IsNullOrEmpty(value)) {
					this.ignoreHttpCodes = "," + value + ",";
				} else {
					this.ignoreHttpCodes = value;
				}
			}
		}
		/// <summary>
		/// 自定义本机地址，此时将输出详细的错误信息
		/// </summary>
		public string CustomLocal {
			get { return this.customLocal; }
			set {
				this.customLocal = value;
				this.customLocals = string.IsNullOrEmpty(value) ? null : value.Split(',');
			}
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
		public override IAOPResult Handle(Exception e, int level) {
			var ex = e as HttpWebException;
			if(ex != null) {
				var app = ex.HttpAppInstance;
				var sourceException = ExceptionBase.FindSourceException(ex);

				//忽略某些状态码，此时不记录日志
				var writeLog = true;
				var ihcs = this.IgnoreHttpCodes;
				if(!string.IsNullOrEmpty(ignoreHttpCodes)) {
					var he = sourceException as HttpException;
					if(he != null) {
						var hc = "," + he.GetHttpCode() + ",";
						if(ihcs.Contains(hc)) {
							writeLog = false;
						}
					}
				}
				if(writeLog) {
					this.LogService.WriteLog(this, level, this.GetFormattedString(sourceException, level, app));
				}
				//是否启用自定义信息
				if(!this.IsCustomEnabled(ex)) {
					goto EXIT;
				}

				app.Server.ClearError();
				app.Response.Clear();
				var fileExists = false;
				if(this.DefaultRedirectUrl != null && this.CheckRedirectFileExists) {
					var filePath = WebHelper.UrlCombine(app.Request.ApplicationPath, this.DefaultRedirectUrl, false);
					fileExists = File.Exists(app.Request.MapPath(filePath));
				}
				if(!fileExists && this.CheckRedirectFileExists) {
					this.LogService.WriteLog(this.GetType(), LogLevel.WARN, "WARNING: The file defined in HttpWebExceptionHandler's defaultRedirectUrl do not exists, please check it!");
				}
				if(!string.IsNullOrEmpty(this.DefaultRedirectUrl) && (fileExists || !this.CheckRedirectFileExists) && !WebHelper.IsUrlEquals(this.DefaultRedirectUrl, app.Request)) {
					app.Response.Redirect(string.Format(this.DefaultRedirectUrl, sourceException.GetHashCode(), 0, level, HttpUtility.UrlEncode(HttpUtility.HtmlEncode(sourceException.Message), app.Request.ContentEncoding), HttpUtility.UrlEncode(HttpUtility.HtmlEncode(app.Request.Url.PathAndQuery), app.Request.ContentEncoding)), true);
				} else {
					app.Response.Write(this.GetDisplayMessage(ex));
					app.CompleteRequest();
				}
			}
			EXIT:
			return AOPResult.Success();
		}

		//获取是否输出自定义信息，可根据访问者有选择的输出
		protected virtual bool IsCustomEnabled(HttpWebException ex) {
			//如果是本电脑访问，则输出详细信息
			if (ex.HttpAppInstance.Request.IsLocal) {
				return false;
			}
			//判断来访者是否为本地
			if (this.customLocals != null && this.customLocals.Length > 0) {
				var ip = ex.HttpAppInstance.Request.UserHostAddress;
				if (this.customLocals.Any(ip.StartsWith)) {//IP地址是本地
					return false;
				}
			}
			return this.CustomEnabled;
		}

		//获取自定义显示的出错信息
		protected virtual string GetDisplayMessage(HttpWebException ex) {
			var e = ex.InnerException as HttpException;
			var statusCode = e != null ? e.GetHttpCode().ToString() : null;//获取HTTP状态
			var message = this.DefaultErrorMessage;
			if (string.IsNullOrEmpty(message)) {
				message = "<h1>ERROR OCCURRED</h1><i>error code:{3}</i>";
			}
			message = string.Format(message, ex.InnerException.Message, ex.HttpAppInstance.Request.Url, ex.InnerException, statusCode);
			return message;
		}
	}
}

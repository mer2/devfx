/* Copyright(c) 2005-2011 R2@DevFx.NET, License(LGPL) */

using System;
using System.IO;
using System.Text;
using System.Web;
using HTB.DevFx.Utils;

namespace HTB.DevFx.Exceptions.Web
{
	/// <summary>
	/// 收集WEB异常的异常收集器
	/// </summary>
	/// <remarks>
	/// 主要收集了异常发生时的如下信息：
	/// <list type="bullet">
	///		<item><description>异常信息</description></item>
	///		<item><description>HTTP请求方式 请求的页面</description></item>
	///		<item><description>请求页面的Hash Code（提供给对照异常使用）</description></item>
	///		<item><description>请求的HTTP头</description></item>
	///		<item><description>异常堆栈</description></item>
	///		<item><description>请求的HTTP BODY</description></item>
	///		<item><description>客户端IP</description></item>
	///		<item><description>服务端IP</description></item>
	///		<item><description>当前登录的用户名</description></item>
	/// </list>
	/// </remarks>
	public class HttpWebExceptionFormatter : IExceptionFormatter
	{
		#region IExceptionFormatter Members

		string IExceptionFormatter.GetFormatString(Exception e, object attachObject) {
			const string formatMessage =
@"{0}
{1} {2}
hash code:{3}
request headers:
{4}
request data:
{6}
client ip:{7}
server name/ip:{8}
auth username:{9}
duration:{10}
error data:
{5}
";
			var app = attachObject as HttpApplication;
			if(app == null) {
				var ctx = HttpContext.Current;
				if(ctx == null) {
					return e.ToString();
				}
				app = ctx.ApplicationInstance;
			}
			var duration = (DateTime.Now - app.Context.Timestamp).TotalMilliseconds;
			var ret = new StringBuilder();
			string body;
			if(app.Request.Files.Count <= 0) {
				var streamReader = new StreamReader(app.Request.InputStream, app.Request.ContentEncoding);
				body = streamReader.ReadToEnd();
			} else {//文件上传不保存请求内容（否则太大了）
				body = WebHelper.NameValueCollectionToQueryString(app.Request.Form, app.Request.ContentEncoding);
			}
			ret.AppendFormat(formatMessage, e.Message, app.Request.HttpMethod, app.Request.Url, e.GetHashCode(), app.Request.Headers, e, body, app.Request.UserHostAddress, app.Server.MachineName, app.User.Identity.Name, duration);
			return ret.ToString();
		}

		#endregion
	}
}


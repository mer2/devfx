/* Copyright(c) 2005-2017 R2@DevFx.NET, License(LGPL) */

using DevFx.Web;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;

namespace DevFx.Utils
{
	/// <summary>
	/// 关于WEB项目的一些实用方法
	/// </summary>
	public static class WebHelper
	{
		#region Url

		/// <summary>
		/// Url是否相同（QueryString可不同）
		/// </summary>
		/// <param name="expectedUrl">被比较的Url</param>
		/// <param name="url">比较的Url</param>
		/// <returns>相同返回true</returns>
		public static bool IsUrlEquals(string expectedUrl, string url) {
			var index = url.LastIndexOf('?');
			if(index >= 0) {
				url = url.Substring(0, index);
			}
			return string.Compare(url, expectedUrl, StringComparison.InvariantCultureIgnoreCase) == 0;
		}

		/// <summary>
		/// 把指定的Url转换成相对地址（以"~/"打头）
		/// </summary>
		/// <param name="url">被转换的Url</param>
		/// <param name="basePath">基地址（比如虚拟目录的地址：/WebApplication1）</param>
		/// <returns>转换后的相对地址</returns>
		public static string MakeUrlRelative(string url, string basePath) {
			if(string.IsNullOrEmpty(url)) {
				return "~/";
			}
			if(url.StartsWith("~/")) {
				return url;
			}

			if(basePath == null || !basePath.StartsWith("/")) {
				basePath = "/" + basePath;
			}
			if(url.StartsWith("http://", true, null)) {
				var uri = new Uri(url);
				url = uri.PathAndQuery;
			} else if (!url.StartsWith("/")) {
				url = "/" + url;
			}
			if(basePath == "/") {
				return "~" + url;
			}
			basePath = basePath.ToLower();
			var url1 = url.ToLower();
			url = url.Substring(url1.IndexOf(basePath, StringComparison.Ordinal) + basePath.Length);
			if(url.StartsWith("/")) {
				url = "~" + url;
			} else {
				url = "~/" + url;
			}
			return url;
		}

		/// <summary>
		/// 把指定的路径信息和相对地址（以"~/"打头）合并成绝对路径（以"/"打头）
		/// </summary>
		/// <param name="basePath">基地址（比如虚拟目录的地址：/WebApplication1）</param>
		/// <param name="url">被转换的Url</param>
		/// <param name="includeQueryString">是否包含Url参数</param>
		/// <returns>转换后的绝对地址，如果url不是以"~/"打头，则返回原url</returns>
		public static string UrlCombine(string basePath, string url, bool includeQueryString) {
			if(basePath == null || !basePath.StartsWith("/")) {
				basePath = "/" + basePath;
			}
			if(basePath.Length > 1 && !basePath.EndsWith("/")) {
				basePath = basePath + "/";
			}
			if(string.IsNullOrEmpty(url)) {
				return basePath;
			}
			if(url.StartsWith("~/")) {
				var length = url.Length - 2;
				if(!includeQueryString) {
					var i = url.IndexOf('?');
					if(i > 2) {
						length = i - 2;
					}
				}
				url = basePath + url.Substring(2, length);
			}
			return url;
		}

		/// <summary>
		/// 把指定的Url信息和相对地址（以"~/"打头）合并成绝对路径（以“http://”或“/”打头）
		/// </summary>
		/// <param name="baseUrl">基地址（绝对或相对）</param>
		/// <param name="relativeUrl">相对地址</param>
		/// <param name="includeQueryString">是否包含Url参数</param>
		/// <returns>转换后的绝对地址</returns>
		public static string UriCombine(string baseUrl, string relativeUrl, bool includeQueryString) {
			return UriCombine(baseUrl, relativeUrl, includeQueryString, UriKind.RelativeOrAbsolute);
		}

		/// <summary>
		/// 把指定的Url信息和相对地址（以"~/"打头）合并成绝对路径（以“http://”或“/”打头）
		/// </summary>
		/// <param name="baseUrl">基地址（绝对或相对）</param>
		/// <param name="relativeUrl">相对地址</param>
		/// <param name="includeQueryString">是否包含Url参数</param>
		/// <param name="expectedUriKind">期望的Uri类型</param>
		/// <returns>转换后的绝对地址</returns>
		public static string UriCombine(string baseUrl, string relativeUrl, bool includeQueryString, UriKind expectedUriKind) {
			if(string.IsNullOrEmpty(baseUrl) && string.IsNullOrEmpty(relativeUrl)) {
				return null;
			}
			if(!string.IsNullOrEmpty(relativeUrl) && relativeUrl.StartsWith("~/")) {
				relativeUrl = relativeUrl.Substring(2);
			}
			if(string.IsNullOrEmpty(baseUrl)) {
				return relativeUrl;
			}
			if(expectedUriKind == UriKind.Relative || Uri.IsWellFormedUriString(baseUrl, UriKind.Relative)) {
				return UrlCombine(baseUrl, relativeUrl, includeQueryString);
			}
			if(!Uri.TryCreate(baseUrl, expectedUriKind, out var baseUri)) {
				return relativeUrl;
			}
			return Uri.TryCreate(baseUri, relativeUrl, out var uri) ? uri.OriginalString : baseUri.OriginalString;
		}

		/// <summary>
		/// 名/值集合转换成QueryString形式
		/// 若要反向转换，请使用<see cref="HttpUtility.ParseQueryString(string, System.Text.Encoding)"/>
		/// </summary>
		/// <param name="collection">名/值集合</param>
		/// <param name="encoding">编码</param>
		/// <returns>QueryString</returns>
		public static string NameValueCollectionToQueryString(NameValueCollection collection, Encoding encoding) {
			if(collection == null || collection.Count <= 0) {
				return string.Empty;
			}
			var sb = new StringBuilder();
			foreach (string name in collection.Keys) {
				var value = collection[name];
				sb.AppendFormat("&{0}={1}", HttpUtility.UrlEncode(name, encoding), HttpUtility.UrlEncode(value, encoding));
			}
			return sb.Remove(0, 1).ToString();
		}

		/// <summary>
		/// 名/值集合转换成QueryString形式
		/// 若要反向转换，请使用<see cref="HttpUtility.ParseQueryString(string, System.Text.Encoding)"/>
		/// </summary>
		/// <param name="collection">名/值集合</param>
		/// <param name="encodingName">编码名称</param>
		/// <returns>QueryString</returns>
		public static string NameValueCollectionToQueryString(NameValueCollection collection, string encodingName) {
			return NameValueCollectionToQueryString(collection, Encoding.GetEncoding(encodingName));
		}

		public static bool IsTrustedUrl(string url, string[] domains) {
			if(string.IsNullOrEmpty(url)) {
				return false;
			}
			if(!Uri.TryCreate(url, UriKind.Absolute, out var uri)) {
				return false;
			}
			if(domains == null || domains.Length <= 0) {
				return false;
			}
			var host = "." + uri.Host;
			return domains.Any(dm => host.EndsWith("." + dm, StringComparison.InvariantCultureIgnoreCase));
		}

		#endregion Url

		#region Base64

		/// <summary>
		/// 把字符串进行BASE64编码
		/// </summary>
		/// <param name="inputString">原字符串</param>
		/// <param name="encodingName">编码格式名</param>
		/// <returns>编码后的字符串</returns>
		public static string ToBase64(string inputString, string encodingName) {
			return Convert.ToBase64String(Encoding.GetEncoding(encodingName).GetBytes(inputString));
		}

		/// <summary>
		/// 对BASE64字符串进行解码
		/// </summary>
		/// <param name="base64String">待解码的字符串</param>
		/// <param name="encodingName">编码格式名</param>
		/// <returns>解码后的字符串</returns>
		public static string FromBase64(string base64String, string encodingName) {
			return Encoding.GetEncoding(encodingName).GetString(Convert.FromBase64String(base64String));
		}

		#endregion Base64

		#region Hex

		private static int ConvertHexDigit(char val) {
			if(val <= '9') {
				return (val - '0');
			}
			if(val >= 'a') {
				return ((val - 'a') + '\n');
			}
			return ((val - 'A') + '\n');
		}

		/// <summary>
		/// 把字节数组转换成16进制表示的字符串
		/// </summary>
		/// <param name="sArray">字节数组</param>
		/// <returns>16进制表示的字符串</returns>
		public static string ToHexString(byte[] sArray) {
			if(sArray == null) {
				return null;
			}
			var sb = new StringBuilder(sArray.Length * 2);
			foreach(var b in sArray) {
				sb.Append(b.ToString("X2"));
			}
			return sb.ToString();
		}

		/// <summary>
		/// 把16进制表示的字符串转换成字节数组
		/// </summary>
		/// <param name="hexString">进制表示的字符串</param>
		/// <returns>字节数组</returns>
		public static byte[] FromHexString(string hexString) {
			byte[] buffer;
			if(hexString == null) {
				throw new ArgumentNullException(nameof(hexString));
			}
			bool flag = false;
			int i = 0;
			int hexLength = hexString.Length;
			if(((hexLength >= 2) && (hexString[0] == '0')) && ((hexString[1] == 'x') || (hexString[1] == 'X'))) {
				hexLength = hexString.Length - 2;
				i = 2;
			}
			if(((hexLength % 2) != 0) && ((hexLength % 3) != 2)) {
				throw new ArgumentException("无效的16进制字符串格式");
			}
			if((hexLength >= 3) && (hexString[i + 2] == ' ')) {
				flag = true;
				buffer = new byte[(hexLength / 3) + 1];
			} else {
				buffer = new byte[hexLength / 2];
			}
			for(int k = 0; i < hexString.Length; k++) {
				int h = ConvertHexDigit(hexString[i]);
				int l = ConvertHexDigit(hexString[i + 1]);
				buffer[k] = (byte)(l | (h << 4));
				if(flag) {
					i++;
				}
				i += 2;
			}
			return buffer;
		}

		#endregion Hex

		#region JSON

		internal static Regex JsonRegex = new Regex(@"(?: *?)(\w+)(?: *?):(?: *?)(\'.+?\'|\"".+?\""|\S+)(?: *?)", RegexOptions.Compiled);
		internal static T FromJsonLite<T>(string json, Action<T, string, string> initializer) where T : new() {
			if (string.IsNullOrEmpty(json) || !json.StartsWith("{") || !json.EndsWith("}")) {
				return default;
			}
			var instance = new T();
			if(initializer == null) {
				return instance;
			}
			json = json.TrimStart('{').TrimEnd('}');
			var matches = JsonRegex.Matches(json);
			foreach(Match match in matches) {
				if(!match.Success || match.Groups.Count != 3) {
					continue;
				}
				var name = match.Groups[1].Value.Trim();
				var value = match.Groups[2].Value.Trim(' ', '\'', '\"');
				initializer(instance, name, value);
			}
			return instance;
		}

		#endregion

		#region Web

		public static string GetRequestDomain(HttpContext ctx = null, bool main = true, string[] headers = null) {
			if (ctx == null) {
				ctx = HttpContextHolder.Current;
			}
			if (ctx == null) {
				return null;
			}
			var list = new List<string>();
			if (headers != null) {
				list.AddRange(headers);
			} else {
				list.AddRange(new[] { "X-Real-Host", "HOST" });
			}
			string domain = null;
			foreach (var header in list) {
				domain = ctx.Request.Headers[header];
				if (!string.IsNullOrEmpty(domain)) {
					break;
				}
			}
			if (!string.IsNullOrEmpty(domain) && main) {
				//仅处理一级的域名，即结尾为：.com/.cn等
				var parts = domain.Split('.');
				var length = parts.Length;
				if (length > 1) {
					domain = parts[length - 2] + "." + parts[length - 1];
				}
			}
			return domain;
		}

		public static string GetRequestIPAddress(HttpContext ctx = null, string[] headers = null) {
			if (ctx == null) {
				ctx = HttpContextHolder.Current;
			}
			if (ctx == null) {
				return null;
			}
			var list = new List<string>();
			if (headers != null) {
				list.AddRange(headers);
			} else {
				list.AddRange(new[] { "X-Real-IP" });
			}
			string ip = null;
			foreach (var header in list) {
				ip = ctx.Request.Headers[header];
				if (!string.IsNullOrEmpty(ip)) {
					break;
				}
			}
			if(string.IsNullOrEmpty(ip)) {
				var ipa = ctx.Connection.RemoteIpAddress;
				if (ipa != null) {
					if (ipa.IsIPv4MappedToIPv6) {
						ipa = ipa.MapToIPv4();
					}
					ip = ipa.ToString();
				}
			}
			return ip;
		}

		#endregion
	}
}
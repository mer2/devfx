using System;
using System.Collections;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;

namespace DevFx.Utils
{
	public static partial class StringHelper
	{
		private static readonly char[] Separator = { ':' };

		public static string NamedStringFormat(this string format, object args) {
			return format.NamedStringFormat(args.ToDictionary());
		}

		public static string NamedStringFormat(this string format, IDictionary args, Func<string[], object, string> matchedHandle = null, char startChar = '{', char endChar = '}') {
			if (string.IsNullOrEmpty(format)) {
				return format;
			}
			var regexString = $"\\{startChar}(?<param>.*?)\\{endChar}";
			var regexFind = new Regex(regexString, RegexOptions.Compiled | RegexOptions.Singleline);
			matchedHandle = matchedHandle ?? MatchedHandle;
			//把包围的字符临时替换成不可见的字符
			format = format.Replace($"{startChar}{startChar}", "\x2").Replace($"{endChar}{endChar}", "\x3");
			var result = regexFind.Replace(format, delegate(Match match) {
				var param0 = match.Groups["param"].Value;
				var param = param0.Split(Separator);
				var value = args.Contains(param[0]) ? args[param[0]] : null;
				return matchedHandle(param, value);
			});
			result = result.Replace('\x2', startChar).Replace('\x3', endChar);
			return result;
		}

		public static string MatchedHandle(string[] param, object value) {
			if(param.Length <= 1 || string.IsNullOrEmpty(param[1])) {//只有一个参数或第二个参数为空，直接返回
				return Convert.ToString(value);
			}
			//Url编码
			if(string.Compare(param[1], "urlencode", StringComparison.InvariantCultureIgnoreCase) == 0) {
				var str = Convert.ToString(value);
				if(string.IsNullOrEmpty(str)) {
					return str;
				}
				string encodingName = null;
				if(param.Length > 2) {
					encodingName = param[2];
				}
				if(string.IsNullOrEmpty(encodingName)) {
					encodingName = "utf-8";
				}
				var encoding = Encoding.GetEncoding(encodingName);
				return HttpUtility.UrlEncode(str, encoding);
			}
			//JSON转换
			if(string.Compare(param[1], "json", StringComparison.InvariantCultureIgnoreCase) == 0) {
				var str = Convert.ToString(value);
				if(string.IsNullOrEmpty(str)) {
					return str;
				}
				str = str.Replace("\"", "\\\"");
				return str;
			}
			//金额分转换
			if(string.Compare(param[1], "amount", StringComparison.InvariantCultureIgnoreCase) == 0) {
				var str = Convert.ToInt64(value) / 100D;
				return $"{str:0.00}";
			}
			//有多个参数，把第一个参数换为0
			param[0] = "0";
			return string.Format(CultureInfo.CurrentCulture, "{" + string.Join(":", param) + "}", value);
		}
	}
}
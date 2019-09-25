using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Web;

namespace DevFx.Utils
{
	public static class DictionaryHelper
	{
		/// <summary>
		/// 把对象转换成Key/Value形式
		/// </summary>
		/// <param name="parameters">对象</param>
		/// <returns><see cref="IDictionary"/></returns>
		public static IDictionary ToDictionary(this object parameters) {
			return parameters.ToDictionary(false);
		}

		/// <summary>
		/// 把对象转换成Key/Value形式
		/// </summary>
		/// <param name="parameters">对象</param>
		/// <param name="reflectionOnly">是否仅用反射方式</param>
		/// <returns><see cref="IDictionary"/></returns>
		public static IDictionary ToDictionary(this object parameters, bool reflectionOnly) {
			if (!reflectionOnly) {
				if (parameters == null) {
					return new Dictionary<string, object>();
				}
				if (parameters is Dictionary<string, object>) {
					return (Dictionary<string, object>)parameters;
				}
				if (parameters is IDictionary) {
					return (IDictionary)parameters;
				}
				if (parameters is IDictionary<string, object>) {
					return new Dictionary<string, object>((IDictionary<string, object>)parameters);
				}
				//TODO
				/*
				if (parameters is IEntity) {
					return ((IEntity)parameters).GetChangedValues();
				}*/
				if (parameters is IDataRecord) {
					return ((IDataRecord)parameters).ToDictionary();
				}
				if (parameters is NameValueCollection nv) {
					var dict = new Dictionary<string, object>();
					foreach (var key in nv.AllKeys) {
						dict.Add(key, nv[key]);
					}
					return dict;
				}
			}
			if(parameters != null) {
				var properties = TypeHelper.GetPropertyAccessors(parameters.GetType());
				return properties.ToDictionary(k => k.Key, v => v.Value.GetValue(parameters));
			}
			return new Dictionary<string, object>();
		}

		/// <summary>
		/// 把<see cref="IDataRecord"/>转换成<see cref="IDictionary"/>
		/// </summary>
		/// <param name="parameters"><see cref="IDataRecord"/></param>
		/// <returns><see cref="IDictionary"/></returns>
		public static IDictionary ToDictionary(this IDataRecord parameters) {
			return parameters.ToDictionary(null);
		}
	
		/// <summary>
		/// 把<see cref="IDataRecord"/>转换成<see cref="Dictionary{TKey,TValue}"/>
		/// </summary>
		/// <param name="parameters"><see cref="IDataRecord"/></param>
		/// <param name="comparer">字符串比较器</param>
		/// <returns><see cref="Dictionary{TKey,TValue}"/></returns>
		public static Dictionary<string, object> ToDictionary(this IDataRecord parameters, IEqualityComparer<string> comparer) {
			var dict = new Dictionary<string, object>(comparer);
			if (parameters != null) {
				for (var i = 0; i < parameters.FieldCount; i++) {
					var columnName = parameters.GetName(i);
					var columnValue = parameters.GetValue(i);
					dict.Add(columnName, columnValue);
				}
			}
			return dict;
		}

		/// <summary>
		/// 把<see cref="DbParameterCollection"/>转换成<see cref="IDictionary"/>
		/// </summary>
		/// <param name="parameters"><see cref="DbParameterCollection"/></param>
		/// <returns><see cref="IDictionary"/></returns>
		public static IDictionary ToDictionary(this DbParameterCollection parameters) {
			var dict = new Dictionary<string, object>();
			if (parameters != null) {
				for (var i = 0; i < parameters.Count; i++) {
					var parameter = parameters[i];
					var columnName = parameter.ParameterName;
					var columnValue = parameter.Value;
					dict.Add(columnName, columnValue);
				}
			}
			return dict;
		}

		/// <summary>
		/// 把<see cref="IDictionary"/>内的键值序列化成字符串
		/// </summary>
		/// <param name="parameters"><see cref="IDictionary"/></param>
		/// <param name="encoding">字符编码</param>
		/// <returns>序列化后的字符串</returns>
		public static string ToString(this IDictionary parameters, Encoding encoding) {
			return parameters.ToString(null, encoding);
		}

		/// <summary>
		/// 把<see cref="IDictionary"/>内的键值序列化成字符串
		/// </summary>
		/// <param name="parameters"><see cref="IDictionary"/></param>
		/// <param name="keys">需要序列化的键</param>
		/// <param name="encoding">字符编码</param>
		/// <returns>序列化后的字符串</returns>
		public static string ToString(this IDictionary parameters, object[] keys, Encoding encoding) {
			if (parameters == null || parameters.Count <= 0) {
				return string.Empty;
			}
			if (encoding == null) {
				encoding = Encoding.UTF8;
			}
			var dict = new Dictionary<string, object>();
			if (keys != null) {
				foreach (var key in keys) {
					dict.Add(Convert.ToString(key), key);
				}
			} else {
				foreach(var key in parameters.Keys) {
					dict.Add(Convert.ToString(key), key);
				}
			}
			dict.OrderBy(x => x.Key);
			var sb = new StringBuilder();
			foreach (var name in dict.Keys) {
				var value = Convert.ToString(parameters[dict[name]]);
				sb.AppendFormat("&{0}={1}", HttpUtility.UrlEncode(name, encoding), HttpUtility.UrlEncode(value, encoding));
			}
			return sb.Remove(0, 1).ToString();
		}

		/// <summary>
		/// 把经过<see cref="ToString(IDictionary, Encoding)" />转换的字符串转化成<see cref="IDictionary"/>
		/// </summary>
		/// <param name="serializeInfo">序列化后的字符串</param>
		/// <param name="encoding">字符编码</param>
		/// <returns><see cref="IDictionary"/></returns>
		public static IDictionary ToDictionary(this string serializeInfo, Encoding encoding) {
			var dict = new Dictionary<string, object>();
			if(string.IsNullOrEmpty(serializeInfo)) {
				return dict;
			}
			if(encoding == null) {
				encoding = Encoding.UTF8;
			}
			var nv = HttpUtility.ParseQueryString(serializeInfo, encoding);
			return nv.ToDictionary();
		}
		/// <summary>
		/// 合并
		/// </summary>
		public static IDictionary Merge(this IDictionary dict, IDictionary options) {
			if(dict == null || options == null || options.Count <= 0) {
				return dict;
			}
			foreach(var key in options.Keys) {
				dict[key] = options[key];
			}
			return dict;
		}
	}
}

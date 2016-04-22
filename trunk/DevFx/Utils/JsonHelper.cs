/* Copyright(c) 2005-2011 R2@DevFx.NET, License(LGPL) */

using System.Web.Script.Serialization;

namespace HTB.DevFx.Utils
{
	public static class JsonHelper
	{
		#region JSON

		/// <summary>
		/// 把对象转换成JSON格式串
		/// </summary>
		/// <param name="instance">对象实例</param>
		/// <param name="throwOnError">转换失败时是否抛出异常</param>
		/// <returns>JSON格式串</returns>
		public static string ToJson(object instance, bool throwOnError) {
			try {
				var js = new JavaScriptSerializer();
				return js.Serialize(instance);
			} catch {
				if (throwOnError) {
					throw;
				}
			}
			return null;
		}

		/// <summary>
		/// 把JSON格式串转换成对象
		/// </summary>
		/// <typeparam name="T">对象类型</typeparam>
		/// <param name="json">JSON格式串</param>
		/// <param name="throwOnError">转换失败时是否抛出异常</param>
		/// <returns>对象实例</returns>
		public static T FromJson<T>(string json, bool throwOnError) {
			return FromJson(json, default(T), throwOnError);
		}

		/// <summary>
		/// 把JSON格式串转换成对象
		/// </summary>
		/// <typeparam name="T">对象类型</typeparam>
		/// <param name="json">JSON格式串</param>
		/// <param name="defaultValue">转换失败后默认的对象实例</param>
		/// <param name="throwOnError">转换失败时是否抛出异常</param>
		/// <returns>对象实例</returns>
		public static T FromJson<T>(string json, T defaultValue, bool throwOnError) {
			try {
				var js = new JavaScriptSerializer();
				return js.Deserialize<T>(json);
			} catch {
				if (throwOnError) {
					throw;
				}
			}
			return defaultValue;
		}

		#endregion
	}
}

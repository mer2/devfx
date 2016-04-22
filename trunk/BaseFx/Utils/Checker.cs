/* Copyright(c) 2005-2011 R2@DevFx.NET, License(LGPL) */

using System;

namespace HTB.DevFx.Utils
{
	/// <summary>
	/// 关于参数验证的一些实用方法
	/// </summary>
	public static class Checker
	{
		/// <summary>
		/// 检查字符串是否为空（null或者长度为0）
		/// </summary>
		/// <param name="argName">字符串名</param>
		/// <param name="argValue">被检查的字符串</param>
		/// <param name="throwError">为空时是否抛出异常</param>
		/// <returns>为空则返回true</returns>
		public static bool CheckEmptyString(string argName, string argValue, bool throwError) {
			CheckArgumentNull("argName", argName, true);
			bool ret = string.IsNullOrEmpty(argValue);
			if(ret && throwError) {
				throw new ArgumentException("字符串为空", argName);
			}
			return ret;
		}

		/// <summary>
		/// 检查参数是否为空引用（null）
		/// </summary>
		/// <param name="argName">参数名</param>
		/// <param name="argValue">被检查的参数</param>
		/// <param name="throwError">为空引用时是否抛出异常</param>
		/// <returns>为空则返回true</returns>
		public static bool CheckArgumentNull(string argName, object argValue, bool throwError) {
			if(argName == null) {
				throw new ArgumentNullException("argName");
			}
			if(argValue == null) {
				if(throwError) {
					throw new ArgumentNullException(argName);
				}
				return true;
			}
			return false;
		}

		/// <summary>
		/// 检查数组是否为空（长度为0）
		/// </summary>
		/// <param name="argName">数组名</param>
		/// <param name="argValue">被检查的数组实例</param>
		/// <param name="throwError">为空引用时是否抛出异常</param>
		/// <returns>为空则返回true</returns>
		public static bool CheckEmptyArray(string argName, Array argValue, bool throwError) {
			if(argName == null) {
				throw new ArgumentNullException("argName");
			}
			bool ret = (argValue == null || argValue.Length == 0);
			if(ret && throwError) {
				throw new ArgumentException("数组为空", argName);
			}
			return ret;
		}

		/// <summary>
		/// 判断某值是否在枚举内（位枚举）
		/// </summary>
		/// <param name="checkingValue">被检测的枚举值</param>
		/// <param name="expectedValue">期望的枚举值</param>
		/// <returns>是否包含</returns>
		public static bool CheckFlagsEnumEquals(Enum checkingValue, Enum expectedValue) {
			int intCheckingValue = Convert.ToInt32(checkingValue);
			int intExpectedValue = Convert.ToInt32(expectedValue);
			return (intCheckingValue & intExpectedValue) == intExpectedValue;
		}
	}
}

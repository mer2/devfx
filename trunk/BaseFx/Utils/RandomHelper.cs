/* Copyright(c) 2005-2011 R2@DevFx.NET, License(LGPL) */

using System.Security.Cryptography;

namespace HTB.DevFx.Utils
{
	/// <summary>
	/// 随机数实用类
	/// </summary>
	public static class RandomHelper
	{
		/// <summary>
		/// 获取随机字节序列
		/// </summary>
		/// <param name="length">字节序列的长度</param>
		/// <returns>字节序列</returns>
		public static byte[] GetRandomBytes(int length) {
			return GetRandomBytes(length, false);
		}
	
		/// <summary>
		/// 获取随机字节序列
		/// </summary>
		/// <param name="length">字节序列的长度</param>
		/// <param name="nonZero">生成的数字是否可为0</param>
		/// <returns>字节序列</returns>
		public static byte[] GetRandomBytes(int length, bool nonZero) {
			if (length <= 0) {
				return new byte[0];
			}
			var rng = new RNGCryptoServiceProvider();
			var ret = new byte[length];
			if(nonZero) {
				rng.GetNonZeroBytes(ret);
			} else {
				rng.GetBytes(ret);
			}
			return ret;
		}

		/// <summary>
		/// 缺省的字符串取值范围
		/// </summary>
		public const string DEFAULT_CHARLIST = "!\"#$%&'()*+,-./0123456789:;<=>?@ABCDEFGHIJKLMNOPQRSTUVWXYZ[\\]^_`abcdefghijklmnopqrstuvwxyz{|}~";
		/// <summary>
		/// 可读的字符串取值范围
		/// </summary>
		public const string READ_CHARLIST  = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz";

		/// <summary>
		/// 获取随机字符串
		/// </summary>
		/// <param name="length">字符串长度</param>
		/// <param name="charList">字符串取值范围（如果为Null或为空，则返回空字符串）</param>
		/// <returns>随机字符串</returns>
		public static string GetRandomString(int length, string charList) {
			if(length <= 0 || Checker.CheckEmptyString("charList", charList, false)) {
				return string.Empty;
			}
			var num = charList.Length;
			var ret = new char[length];
			var rnd = GetRandomBytes(length);
			for(var i = 0; i < rnd.Length; i++) {
				ret[i] = charList[rnd[i] % num];
			}
			return new string(ret);
		}

		/// <summary>
		/// 获取随机字符串
		/// </summary>
		/// <param name="length">字符串长度</param>
		/// <returns>随机字符串</returns>
		/// <remarks>
		/// 缺省使用ASCII从33到126共94个字符作为取值范围
		/// </remarks>
		public static string GetRandomString(int length) {
			return GetRandomString(length, DEFAULT_CHARLIST);
		}
	}
}

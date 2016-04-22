/* Copyright(c) 2005-2011 R2@DevFx.NET, License(LGPL) */

using System.Web.Security;

namespace HTB.DevFx.Utils
{
	/// <summary>
	/// 关于数据加解密的一些实用方法
	/// </summary>
	/// <remarks>
	/// 更多加解密实用方法请参见 <seealso cref="HTB.DevFx.Cryptography"/> 命名空间
	/// </remarks>
	public static class Encryption
	{
		/// <summary>
		/// Hash算法，提供MD5、SHA1算法
		/// </summary>
		/// <param name="encryptingString">被Hash的字符串</param>
		/// <param name="encryptFormat">Hash算法，有"md5"、"sha1"、"clear"（明文，即不加密）等</param>
		/// <returns>Hash结果字符串</returns>
		/// <remarks>
		/// 当参数<paramref name="encryptFormat" />不为"md5"、"sha1"、"clear"时，直接返回参数<paramref name="encryptingString" />
		/// </remarks>
		public static string Encrypt(string encryptingString, string encryptFormat) {
			if(string.Compare(encryptFormat, "md5", true) == 0 || string.Compare(encryptFormat, "sha1", true) == 0) {
				return FormsAuthentication.HashPasswordForStoringInConfigFile(encryptingString, encryptFormat);
			}
			return encryptingString;
		}
	}
}
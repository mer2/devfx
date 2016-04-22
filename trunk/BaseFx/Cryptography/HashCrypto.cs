/* Copyright(c) 2005-2011 R2@DevFx.NET, License(LGPL) */

using System.IO;
using System.Security.Cryptography;
using HTB.DevFx.Utils;

namespace HTB.DevFx.Cryptography
{
	/// <summary>
	/// 关于Hash的一些实用方法
	/// </summary>
	public static class HashCrypto
	{
		/// <summary>
		/// Hash算法
		/// </summary>
		/// <param name="input">被Hash的字节数组</param>
		/// <param name="hashFormat">Hash算法："md5"、"sha1"</param>
		/// <returns>Hash结果字节数组</returns>
		/// <remarks>
		/// 当参数<paramref name="hashFormat">不为"md5"、"sha1"时，返回<c>null</c></paramref>
		/// </remarks>
		public static byte[] Hash(byte[] input, string hashFormat) {
			HashAlgorithm algorithm = null;
			if(string.Compare(hashFormat, "sha1", true) == 0) {
				algorithm = SHA1.Create();
			} else if(string.Compare(hashFormat, "md5", true) == 0) {
				algorithm = MD5.Create();
			}
			byte[] result = null;
			if(algorithm != null) {
				result = algorithm.ComputeHash(input);
			}
			return result;
		}

		/// <summary>
		/// Hash算法
		/// </summary>
		/// <param name="input">被Hash的字节流</param>
		/// <param name="hashFormat">Hash算法："md5"、"sha1"</param>
		/// <returns>Hash结果字节数组</returns>
		/// <remarks>
		/// 当参数<paramref name="hashFormat">不为"md5"、"sha1"时，返回<c>null</c></paramref>
		/// </remarks>
		public static byte[] Hash(Stream input, string hashFormat) {
			HashAlgorithm algorithm = null;
			if(string.Compare(hashFormat, "sha1", true) == 0) {
				algorithm = SHA1.Create();
			} else if(string.Compare(hashFormat, "md5", true) == 0) {
				algorithm = MD5.Create();
			}
			byte[] result = null;
			if(algorithm != null) {
				result = algorithm.ComputeHash(input);
			}
			return result;
		}

		/// <summary>
		/// Hash文件
		/// </summary>
		/// <param name="fileName">被Hash的文件（包括路径）</param>
		/// <param name="hashFormat">Hash算法："md5"、"sha1"</param>
		/// <returns>Hash结果字符串</returns>
		/// <remarks>
		/// 当参数<paramref name="hashFormat">不为"md5"、"sha1"时，返回<c>null</c></paramref>
		/// </remarks>
		public static string HashFile(string fileName, string hashFormat) {
			var hashBytes = HashFileReturnRawData(fileName, hashFormat);
			return hashBytes == null ? null : WebHelper.ToHexString(hashBytes);
		}

		/// <summary>
		/// Hash文件
		/// </summary>
		/// <param name="fileName">被Hash的文件（包括路径）</param>
		/// <param name="hashFormat">Hash算法："md5"、"sha1"</param>
		/// <returns>Hash结果</returns>
		/// <remarks>
		/// 当参数<paramref name="hashFormat">不为"md5"、"sha1"时，返回<c>null</c></paramref>
		/// </remarks>
		public static byte[] HashFileReturnRawData(string fileName, string hashFormat) {
			using(var fs = File.OpenRead(fileName)) {
				return Hash(fs, hashFormat);
			}
		}
	}
}
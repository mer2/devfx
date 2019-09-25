/* Copyright(c) 2005-2011 R2@DevFx.NET, License(LGPL) */

using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace DevFx.Utils
{
	/// <summary>
	/// 关于Hash的一些实用方法
	/// </summary>
	public static class HashHelper
	{
		public static HashAlgorithm CreateHashAlgorithm(string hashName) {
			if(!string.IsNullOrEmpty(hashName)) {
				hashName = hashName.ToLowerInvariant();
			}
			HashAlgorithm hash;
			switch(hashName) {
				case "sha1":
					hash = SHA1.Create();
					break;
				case "sha256":
					hash = SHA256.Create();
					break;
				case "sha384":
					hash = SHA384.Create();
					break;
				case "sha512":
					hash = SHA512.Create();
					break;
				default:
					hash = MD5.Create();
					break;
			}
			return hash;
		}

		public static string Hash(string input, string hashName, Encoding encoding = null) {
			using (var hash = CreateHashAlgorithm(hashName)) {
				if (encoding == null) {
					encoding = Encoding.UTF8;
				}
				var bytes = hash.ComputeHash(encoding.GetBytes(input));
				return WebHelper.ToHexString(bytes).ToLowerInvariant();
			}
		}

		/// <summary>
		/// Hash算法
		/// </summary>
		/// <param name="input">被Hash的字节数组</param>
		/// <param name="hashName">Hash算法："md5"、"sha1"，默认为"md5"</param>
		/// <returns>Hash结果字节数组</returns>
		public static byte[] Hash(byte[] input, string hashName) {
			using (var hash = CreateHashAlgorithm(hashName)) {
				return hash.ComputeHash(input);
			}
		}

		/// <summary>
		/// Hash算法
		/// </summary>
		/// <param name="input">被Hash的字节流</param>
		/// <param name="hashName">Hash算法："md5"、"sha1"，默认为"md5"</param>
		/// <returns>Hash结果字节数组</returns>
		public static byte[] Hash(Stream input, string hashName) {
			using(var hash = CreateHashAlgorithm(hashName)) {
				return hash.ComputeHash(input);
			}
		}

		/// <summary>
		/// Hash文件
		/// </summary>
		/// <param name="fileName">被Hash的文件（包括路径）</param>
		/// <param name="hashName">Hash算法："md5"、"sha1"，默认为"md5"</param>
		/// <returns>Hash结果字符串</returns>
		public static string HashFile(string fileName, string hashName) {
			var hashBytes = HashFileReturnRawData(fileName, hashName);
			return hashBytes == null ? null : WebHelper.ToHexString(hashBytes);
		}

		/// <summary>
		/// Hash文件
		/// </summary>
		/// <param name="fileName">被Hash的文件（包括路径）</param>
		/// <param name="hashName">Hash算法："md5"、"sha1"，默认为"md5"</param>
		/// <returns>Hash结果</returns>
		public static byte[] HashFileReturnRawData(string fileName, string hashName) {
			using(var fs = File.OpenRead(fileName)) {
				return Hash(fs, hashName);
			}
		}
	}
}
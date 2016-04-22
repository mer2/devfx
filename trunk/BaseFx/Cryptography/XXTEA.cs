/* Copyright(c) 2005-2011 R2@DevFx.NET, License(LGPL) */

using System;
using System.Text;
using HTB.DevFx.Utils;

namespace HTB.DevFx.Cryptography
{
	/// <summary>
	/// XXTEA加解密算法
	/// </summary>
	public static class XXTEA
	{
		/// <summary>
		/// 加密
		/// </summary>
		/// <param name="data">原文</param>
		/// <param name="key">密钥</param>
		/// <returns>密文</returns>
		/// <remarks>
		/// 密文包含原始数据长度
		/// </remarks>
		public static byte[] Encrypt(byte[] data, byte[] key) {
			return Encrypt(data, key, true);
		}

		/// <summary>
		/// 加密
		/// </summary>
		/// <param name="data">原文</param>
		/// <param name="key">密钥</param>
		/// <param name="includeDataLength">密文是否包含原始数据长度</param>
		/// <returns>密文</returns>
		public static byte[] Encrypt(byte[] data, byte[] key, bool includeDataLength) {
			if (data.Length == 0) {
				return data;
			}
			return ToByteArray(Encrypt(ToUInt32Array(data, includeDataLength), ToUInt32Array(key, false)), false);
		}

		/// <summary>
		/// 加密
		/// </summary>
		/// <param name="data">原文（将经过UTF8编码变换）</param>
		/// <param name="key">密钥（将经过UTF8编码变换及MD5的Hash）</param>
		/// <returns>密文（HEX格式）</returns>
		public static string Encrypt(string data, string key) {
			return WebHelper.ToHexString(Encrypt(Encoding.UTF8.GetBytes(data), HashCrypto.Hash(Encoding.UTF8.GetBytes(key), "md5")));
		}

		/// <summary>
		/// 解密
		/// </summary>
		/// <param name="data">密文</param>
		/// <param name="key">密钥</param>
		/// <returns>原文</returns>
		/// <remarks>
		/// 密文包含原始数据长度
		/// </remarks>
		public static byte[] Decrypt(byte[] data, byte[] key) {
			return Decrypt(data, key, true);
		}

		/// <summary>
		/// 解密
		/// </summary>
		/// <param name="data">密文</param>
		/// <param name="key">密钥</param>
		/// <param name="includeDataLength">密文是否包含原始数据长度</param>
		/// <returns>原文</returns>
		public static byte[] Decrypt(byte[] data, byte[] key, bool includeDataLength) {
			if (data.Length == 0) {
				return data;
			}
			return ToByteArray(Decrypt(ToUInt32Array(data, false), ToUInt32Array(key, false)), includeDataLength);
		}

		/// <summary>
		/// 解密
		/// </summary>
		/// <param name="data">密文（HEX格式）</param>
		/// <param name="key">密钥（将经过UTF8编码变换及MD5的Hash）</param>
		/// <returns>原文</returns>
		public static string Decrypt(string data, string key) {
			return Encoding.UTF8.GetString(Decrypt(WebHelper.FromHexString(data), HashCrypto.Hash(Encoding.UTF8.GetBytes(key), "md5")));
		}

		/// <summary>
		/// 加密
		/// </summary>
		/// <param name="v">原文</param>
		/// <param name="k">密钥</param>
		/// <returns>密文</returns>
		public static uint[] Encrypt(uint[] v, uint[] k) {
			int n = v.Length - 1;
			if (n < 1) {
				return v;
			}
			if (k.Length < 4) {
				uint[] Key = new uint[4];
				k.CopyTo(Key, 0);
				k = Key;
			}
			uint z = v[n], y = v[0], delta = 0x9E3779B9, sum = 0;
			int q = 6 + 52 / (n + 1);
			while (q-- > 0) {
				sum = unchecked(sum + delta);
				uint e = sum >> 2 & 3;
				int p;
				for (p = 0; p < n; p++) {
					y = v[p + 1];
					z = unchecked(v[p] += (z >> 5 ^ y << 2) + (y >> 3 ^ z << 4) ^ (sum ^ y) + (k[p & 3 ^ e] ^ z));
				}
				y = v[0];
				z = unchecked(v[n] += (z >> 5 ^ y << 2) + (y >> 3 ^ z << 4) ^ (sum ^ y) + (k[p & 3 ^ e] ^ z));
			}
			return v;
		}

		/// <summary>
		/// 解密
		/// </summary>
		/// <param name="v">密文</param>
		/// <param name="k">密钥</param>
		/// <returns>原文</returns>
		public static uint[] Decrypt(uint[] v, uint[] k) {
			int n = v.Length - 1;
			if (n < 1) {
				return v;
			}
			if (k.Length < 4) {
				uint[] Key = new uint[4];
				k.CopyTo(Key, 0);
				k = Key;
			}
			uint z = v[n], y = v[0], delta = 0x9E3779B9, sum;
			int q = 6 + 52 / (n + 1);
			sum = unchecked((uint)(q * delta));
			while (sum != 0) {
				uint e = sum >> 2 & 3;
				int p;
				for (p = n; p > 0; p--) {
					z = v[p - 1];
					y = unchecked(v[p] -= (z >> 5 ^ y << 2) + (y >> 3 ^ z << 4) ^ (sum ^ y) + (k[p & 3 ^ e] ^ z));
				}
				z = v[n];
				y = unchecked(v[0] -= (z >> 5 ^ y << 2) + (y >> 3 ^ z << 4) ^ (sum ^ y) + (k[p & 3 ^ e] ^ z));
				sum = unchecked(sum - delta);
			}
			return v;
		}

		private static uint[] ToUInt32Array(byte[] data, bool includeLength) {
			int n = (((data.Length & 3) == 0) ? (data.Length >> 2) : ((data.Length >> 2) + 1));
			uint[] result;
			if (includeLength) {
				result = new uint[n + 1];
				result[n] = (uint)data.Length;
			} else {
				result = new uint[n];
			}
			n = data.Length;
			for (int i = 0; i < n; i++) {
				result[i >> 2] |= (uint)data[i] << ((i & 3) << 3);
			}
			return result;
		}

		private static byte[] ToByteArray(uint[] data, bool includeLength) {
			int n = data.Length << 2;
			if (includeLength) {
				int m = (int)data[data.Length - 1];
				if (m > n) {
					throw new Exception("XXTEA Decrypt Error: Wrong input data.");
				} else {
					n = m;
				}
			}
			byte[] result = new byte[n];
			for (int i = 0; i < n; i++) {
				result[i] = (byte)(data[i >> 2] >> ((i & 3) << 3));
			}
			return result;
		}
	}
}
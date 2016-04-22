/* Copyright(c) 2005-2011 R2@DevFx.NET, License(LGPL) */

using HTB.DevFx.Exceptions;

namespace HTB.DevFx.Cryptography
{
	/// <summary>
	/// 加密异常
	/// </summary>
	public class CryptographyException : ExceptionBase
	{
		/// <summary>
		/// 构造方法
		/// </summary>
		/// <param name="message">异常信息</param>
		public CryptographyException(string message) : base(message) {
		}
	}
}

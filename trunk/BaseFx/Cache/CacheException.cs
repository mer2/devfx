/* Copyright(c) 2005-2011 R2@DevFx.NET, License(LGPL) */

using System;
using HTB.DevFx.Exceptions;

namespace HTB.DevFx.Cache
{
	/// <summary>
	/// 缓存模块异常
	/// </summary>
	/// <remarks>
	/// 在缓存模块里面，能发现的异常都会包装成此类的实例
	/// </remarks>
	[Serializable]
	public class CacheException : ExceptionBase
	{
		/// <summary>
		/// 构造函数
		/// </summary>
		public CacheException() {
		}

		/// <summary>
		/// 构造函数
		/// </summary>
		/// <param name="message">异常消息</param>
		/// <param name="innerException">内部异常</param>
		public CacheException(string message, Exception innerException) : base(message, innerException) {
		}

		/// <summary>
		/// 构造函数
		/// </summary>
		/// <param name="message">异常消息</param>
		public CacheException(string message) : base(message) {
		}

		/// <summary>
		/// 构造函数
		/// </summary>
		/// <param name="errorNo">异常编号</param>
		/// <param name="message">异常消息</param>
		public CacheException(int errorNo, string message) : base(errorNo, message) {
		}

		/// <summary>
		/// 构造函数
		/// </summary>
		/// <param name="errorNo">异常编号</param>
		/// <param name="message">异常消息</param>
		/// <param name="innerException">内部异常</param>
		public CacheException(int errorNo, string message, Exception innerException) : base(errorNo, message, innerException) {
		}
	}
}

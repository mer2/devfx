/* Copyright(c) 2005-2011 R2@DevFx.NET, License(LGPL) */

using System;

namespace DevFx.Logging
{
	/// <inheritdoc />
	/// <summary>
	/// 日志异常
	/// </summary>
	/// <remarks>
	/// 在日志里面，能发现的异常都会包装成此类的实例
	/// </remarks>
	[Serializable]
	public class LogException : Exception
	{
		/// <inheritdoc />
		/// <summary>
		/// 构造函数
		/// </summary>
		public LogException() {
		}

		/// <inheritdoc />
		/// <summary>
		/// 构造函数
		/// </summary>
		/// <param name="message">异常消息</param>
		/// <param name="innerException">内部异常</param>
		public LogException(string message, Exception innerException) : base(message, innerException) {
		}

		/// <inheritdoc />
		/// <summary>
		/// 构造函数
		/// </summary>
		/// <param name="message">异常消息</param>
		public LogException(string message) : base(message) {
		}
	}
}

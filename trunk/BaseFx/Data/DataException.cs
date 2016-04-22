/* Copyright(c) 2005-2011 R2@DevFx.NET, License(LGPL) */

using System;
using HTB.DevFx.Exceptions;

namespace HTB.DevFx.Data
{
	/// <summary>
	/// 数据模块异常
	/// </summary>
	[Serializable]
	public class DataException : ExceptionBase
	{
		/// <summary>
		/// 构造函数
		/// </summary>
		public DataException() {
		}

		/// <summary>
		/// 构造函数
		/// </summary>
		/// <param name="message">异常消息</param>
		/// <param name="innerException">内部异常</param>
		public DataException(string message, Exception innerException) : base(message, innerException) {
		}

		/// <summary>
		/// 构造函数
		/// </summary>
		/// <param name="message">异常消息</param>
		public DataException(string message) : base(message) {
		}

		/// <summary>
		/// 构造函数
		/// </summary>
		/// <param name="errorNo">异常编号</param>
		/// <param name="message">异常消息</param>
		public DataException(int errorNo, string message) : base(errorNo, message) {
		}

		/// <summary>
		/// 构造函数
		/// </summary>
		/// <param name="errorNo">异常编号</param>
		/// <param name="message">异常消息</param>
		/// <param name="innerException">内部异常</param>
		public DataException(int errorNo, string message, Exception innerException) : base(errorNo, message, innerException) {
		}
	}
}

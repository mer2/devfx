/* Copyright(c) 2005-2011 R2@DevFx.NET, License(LGPL) */

using System;

namespace DevFx.Data
{
	/// <summary>
	/// 数据模块异常
	/// </summary>
	[Serializable]
	public class DataException : Exception
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
	}
}

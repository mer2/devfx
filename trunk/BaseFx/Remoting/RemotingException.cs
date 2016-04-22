/* Copyright(c) 2005-2011 R2@DevFx.NET, License(LGPL) */

using System;
using HTB.DevFx.Exceptions;

namespace HTB.DevFx.Remoting
{
	/// <summary>
	/// 远程服务异常类
	/// </summary>
	[Serializable]
	public class RemotingException : ExceptionBase
	{
		/// <summary>
		/// 构造函数
		/// </summary>
		public RemotingException() {
		}

		/// <summary>
		/// 构造函数
		/// </summary>
		/// <param name="message">异常消息</param>
		/// <param name="innerException">内部异常</param>
		public RemotingException(string message, Exception innerException) : base(message, innerException) {
		}

		/// <summary>
		/// 构造函数
		/// </summary>
		/// <param name="message">异常消息</param>
		public RemotingException(string message) : base(message) {
		}

		/// <summary>
		/// 构造函数
		/// </summary>
		/// <param name="errorNo">异常编号</param>
		/// <param name="message">异常消息</param>
		public RemotingException(int errorNo, string message) : base(errorNo, message) {
		}

		/// <summary>
		/// 构造函数
		/// </summary>
		/// <param name="errorNo">异常编号</param>
		/// <param name="message">异常消息</param>
		/// <param name="innerException">内部异常</param>
		public RemotingException(int errorNo, string message, Exception innerException) : base(errorNo, message, innerException) {
		}
	}
}

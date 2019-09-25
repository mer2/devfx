using System;

namespace DevFx
{
	[Serializable]
	public class ResultException : Exception
	{
		/// <summary>
		/// 构造函数
		/// </summary>
		/// <param name="resultNo">异常编号</param>
		/// <param name="message">异常消息</param>
		/// <param name="innerException">内部异常</param>
		public ResultException(int resultNo, string message, Exception innerException = null) : base(message, innerException) {
			this.ResultNo = resultNo;
		}
		/// <summary>
		/// 异常编号
		/// </summary>
		public int ResultNo { get; protected set; }
	}
}

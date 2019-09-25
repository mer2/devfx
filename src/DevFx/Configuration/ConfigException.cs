/* Copyright(c) 2005-2011 R2@DevFx.NET, License(LGPL) */

using System;

namespace DevFx.Configuration
{
	/// <summary>
	/// 配置异常
	/// </summary>
	/// <remarks>
	/// 在配置里面，能发现的异常都会包装成此类的实例
	/// </remarks>
	[Serializable]
	public class ConfigException : Exception
	{
		/// <summary>
		/// 构造函数
		/// </summary>
		public ConfigException() {
		}

		/// <summary>
		/// 构造函数
		/// </summary>
		/// <param name="message">异常消息</param>
		public ConfigException(string message) : base(message) {
		}

		/// <summary>
		/// 构造函数
		/// </summary>
		/// <param name="message">异常消息</param>
		/// <param name="innerException">内部异常</param>
		public ConfigException(string message, Exception innerException) : base(message, innerException) {
		}
	}
}

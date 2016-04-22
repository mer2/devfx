/* Copyright(c) 2005-2011 R2@DevFx.NET, License(LGPL) */

using System;

namespace HTB.DevFx.Exceptions
{
	/// <summary>
	/// 异常信息收集格式化接口，用于收集对异常时的所处的环境信息以及对这些信息进行格式化
	/// </summary>
	public interface IExceptionFormatter
	{
		/// <summary>
		/// 获取格式化的信息
		/// </summary>
		/// <param name="e">发生的异常</param>
		/// <param name="attachObject">附加对象</param>
		/// <returns>格式化后的字符串</returns>
		string GetFormatString(Exception e, object attachObject);
	}
}

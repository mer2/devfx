/* Copyright(c) 2005-2011 R2@DevFx.NET, License(LGPL) */

using System;
using System.Collections;

namespace HTB.DevFx.Data.Entities
{
	/// <summary>
	/// 类型转换接口
	/// </summary>
	/// <remarks>
	/// 可用于数据库数据与.NET数据之间的映射
	/// </remarks>
	public interface ITypeTranslator
	{
		/// <summary>
		/// 类型转换
		/// </summary>
		/// <param name="value">原始类型值</param>
		/// <param name="expectedType">期望的类型值</param>
		/// <param name="options">其他需要传递的参数</param>
		/// <returns>转换后的值</returns>
		object Translate(object value, Type expectedType, IDictionary options);
	}
}

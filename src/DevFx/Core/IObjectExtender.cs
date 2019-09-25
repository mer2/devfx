/* Copyright(c) 2005-2017 R2@DevFx.NET, License(LGPL) */

using System.Collections;

namespace DevFx.Core
{
	/// <summary>
	/// 对象扩展接口
	/// </summary>
	public interface IObjectExtender
	{
		/// <summary>
		/// 初始化
		/// </summary>
		/// <param name="instance">对象实例</param>
		/// <param name="items">其他选项</param>
		void Init(object instance, IDictionary items);
	}

	/// <summary>
	/// 对象扩展接口（泛型）
	/// </summary>
	/// <typeparam name="T">对象类型</typeparam>
	public interface IObjectExtender<in T>
	{
		/// <summary>
		/// 初始化
		/// </summary>
		/// <param name="instance">对象实例</param>
		/// <param name="items">其他选项</param>
		void Init(T instance, IDictionary items);
	}
}

/* Copyright(c) 2005-2011 R2@DevFx.NET, License(LGPL) */

namespace HTB.DevFx.Core
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
		void Init(object instance);
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
		void Init(T instance);
	}
}

/* Copyright(c) 2005-2011 R2@DevFx.NET, License(LGPL) */

using System;

namespace HTB.DevFx.Esb
{
	/// <summary>
	/// 服务定位器，类似IoC
	/// </summary>
	public interface IServiceLocator
	{
		/// <summary>
		/// 获得指定类型服务的实例
		/// </summary>
		/// <typeparam name="T">服务类型</typeparam>
		/// <returns>服务实例</returns>
		T GetService<T>();
	
		/// <summary>
		/// 获得指定类型服务的实例
		/// </summary>
		/// <typeparam name="T">服务类型</typeparam>
		/// <param name="serviceName">实例名称</param>
		/// <returns>服务实例</returns>
		T GetService<T>(string serviceName);

		/// <summary>
		/// 获得指定类型服务的实例
		/// </summary>
		/// <param name="serviceName">实例名称</param>
		/// <returns>服务实例</returns>
		object GetService(string serviceName);

		/// <summary>
		/// 获取类型别名对应的类型名
		/// </summary>
		/// <param name="typeAlias">别名</param>
		/// <returns>类型名</returns>
		string GetTypeName(string typeAlias);

		/// <summary>
		/// 对象创建前事件
		/// </summary>
		event Action<IObjectBuildContext> ObjectBuilding;

		/// <summary>
		/// 对象创建后事件
		/// </summary>
		event Action<IObjectBuildContext> ObjectBuilt;
	}
}
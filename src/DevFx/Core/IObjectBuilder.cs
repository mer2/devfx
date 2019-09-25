/* Copyright(c) 2005-2011 R2@DevFx.NET, License(LGPL) */

using System.Collections;

namespace DevFx.Core
{
	public interface IObjectBuilder
	{
		/// <summary>
		/// 根据<paramref name="objectDescription"/>的配置信息创建对象实例
		/// </summary>
		/// <param name="objectDescription">对象配置信息</param>
		/// <param name="items">其他选项</param>
		/// <param name="args">构造参数</param>
		/// <returns>对象实例</returns>
		object CreateObject(IObjectDescription objectDescription, IDictionary items, params object[] args);
	}
}
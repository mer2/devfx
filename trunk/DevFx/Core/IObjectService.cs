/* Copyright(c) 2005-2011 R2@DevFx.NET, License(LGPL) */

using System;
using System.Collections;
using HTB.DevFx.Config;

namespace HTB.DevFx.Core
{
	/// <summary>
	/// 对象服务接口，核心服务之一，实现IoC Container
	/// </summary>
	public interface IObjectService
	{
		/// <summary>
		/// 根据类型别名获取类型的真实名称
		/// </summary>
		/// <param name="typeAlias">类型别名（配置在配置文件中，包括命名前缀）</param>
		/// <returns>如果有相应的实际类型，则返回实际类型，否则原样返回<paramref name="typeAlias"/></returns>
		string GetTypeName(string typeAlias);

		object GetObject(string objectAlias);
		T GetObject<T>(string objectAlias);

		T GetObject<T>();
		object GetObject(Type type);

		T CreateObject<T>(params object[] args);
		object CreateObject(Type type, params object[] args);
		object CreateObject(string objectAlias, params object[] args);

		IConfigSetting GetObjectSetting<T>();
		IConfigSetting GetObjectSetting(Type type);
		IConfigSetting GetObjectSetting(string objectAlias);

		T[] GetObjects<T>();
		object[] GetObjects(Type type);

		IObjectBuilder ObjectBuilder { get; }

		event Action<IObjectServiceContext> PreInit;
		event Action<IObjectServiceContext> InitCompleted;
		event Action<IObjectServiceContext> Disposing;
		event Action<IObjectServiceContext, Exception> Error;

		#region Advanced Usage

		object GetObject(string objectAlias, IDictionary items);
		T GetObject<T>(string objectAlias, IDictionary items);

		T GetObject<T>(IDictionary items);
		object GetObject(Type type, IDictionary items);

		T CreateObject<T>(IDictionary items, params object[] args);
		object CreateObject(Type type, IDictionary items, params object[] args);
		object CreateObject(string objectAlias, IDictionary items, params object[] args);

		T[] GetObjects<T>(IDictionary items);
		object[] GetObjects(Type type, IDictionary items);

		#endregion

		#region Extension

		IObjectNamespace GetObjectNamespace(string spaceName);

		#endregion
	}
}

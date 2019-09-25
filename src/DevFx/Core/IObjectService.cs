using System;

namespace DevFx.Core
{
	/// <summary>
	/// 对象服务接口，核心服务之一，实现IoC Container
	/// </summary>
	public interface IObjectService
	{
		T GetObject<T>() where T : class;
		T GetObject<T>(string name) where T : class;
		object GetObject(Type type);
		object GetObject(string name);
		T[] GetObjects<T>() where T : class;
		object[] GetObjects(Type type);

		Type GetType(string typeAlias);

		object CreateObject(Type type, params object[] args);
		T CreateObject<T>(params object[] args) where T : class;

		IObjectBuilder ObjectBuilder { get; }

		event Action<IObjectServiceContext> PreInit;
		event Action<IObjectServiceContext> InitCompleted;
		event Action<IObjectServiceContext> SetupCompleted;
		event Action<IObjectServiceContext> Disposing;
		event Action<IObjectServiceContext, Exception> Error;
		event Action<IObjectContainerContext> ObjectContainerGetting;
		event Action<IObjectContainerContext> ObjectContainerGetted;
		event Action<IObjectBuilderContext> ObjectCreating;
		event Action<IObjectBuilderContext> ObjectCreated;
	}
}
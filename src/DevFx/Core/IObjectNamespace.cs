using System;
using System.Collections.Generic;

namespace DevFx.Core
{
	public interface IObjectNamespace
	{
		string Name { get; }
		IDictionary<string, Type> TypeAliases { get; }
		IDictionary<string, object> ConstAliases { get; }
		IDictionary<string, IObjectContainer> ObjectAliases { get; }
		IDictionary<Type, ITypedObjectContainer> TypedObjects { get; }

		IObjectNamespace AddObject(string objectName, IObjectContainer container);
		IObjectNamespace AddObject(Type objectType, IObjectContainer container);
		bool RemoveObject(string objectName);
		bool RemoveObject(Type objectType);
	}

	public interface ITypedObjectContainer
	{
		IObjectContainer Container { get; }
		IList<IObjectContainer> Containers { get; }

		ITypedObjectContainer Add(IObjectContainer container);
	}
}

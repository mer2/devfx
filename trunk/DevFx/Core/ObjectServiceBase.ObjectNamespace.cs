/* Copyright(c) 2005-2011 R2@DevFx.NET, License(LGPL) */

using System;
using System.Collections.Generic;

namespace HTB.DevFx.Core
{
	partial class ObjectServiceBase
	{
		internal class ObjectNamespace
		{
			public ObjectNamespace(string name) {
				this.Name = name;
				this.TypeAliases = new Dictionary<string, string>();
				this.ConstAliases = new Dictionary<string, object>();
				this.ObjectAliases = new Dictionary<string, ILifetimeContainer>();
				this.TypedObjects = new TypedObjectsCollection();
			}

			public string Name { get; private set; }
			public Dictionary<string, string> TypeAliases { get; private set; }
			public Dictionary<string, object> ConstAliases { get; private set; }
			public Dictionary<string, ILifetimeContainer> ObjectAliases { get; private set; }
			public TypedObjectsCollection TypedObjects { get; private set; }
		}

		internal class TypedObjectsCollection : Dictionary<Type, TypedObject>
		{
			public TypedObjectsCollection Add(Type type, ILifetimeContainer container) {
				if (!this.ContainsKey(type)) {
					this.Add(type, new TypedObject());
				}
				this[type].Add(container);
				return this;
			}
		}

		internal class TypedObject
		{
			public ILifetimeContainer Container { get; private set; }
			public List<ILifetimeContainer> Containers { get; private set; }

			public TypedObject Add(ILifetimeContainer container) {
				if(this.Container == null) {
					this.Container = container;
				}
				if(this.Containers == null) {
					this.Containers = new List<ILifetimeContainer>();
				}
				if (!this.Containers.Contains(container)) {
					this.Containers.Add(container);
				}
				return this;
			}
		}
	}
}

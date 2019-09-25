/* Copyright(c) 2005-2011 R2@DevFx.NET, License(LGPL) */

using System;
using System.Collections.Generic;

namespace DevFx.Core
{
	partial class ObjectServiceBase
	{
		internal class ObjectNamespace : IObjectNamespace
		{
			public ObjectNamespace(string name) {
				this.Name = name;
				this.TypeAliases = new Dictionary<string, Type>();
				this.ConstAliases = new Dictionary<string, object>();
				this.ObjectAliases = new Dictionary<string, IObjectContainer>();
				this.TypedObjects = new Dictionary<Type, ITypedObjectContainer>();
			}

			public string Name { get; }
			public IDictionary<string, Type> TypeAliases { get; }
			public IDictionary<string, object> ConstAliases { get; }
			public IDictionary<string, IObjectContainer> ObjectAliases { get; }
			public IDictionary<Type, ITypedObjectContainer> TypedObjects { get; }

			public IObjectNamespace AddObject(string objectName, IObjectContainer container) {
				if(string.IsNullOrEmpty(objectName)) throw new ArgumentNullException(nameof(objectName));
				if(container == null) throw new ArgumentNullException(nameof(container));

				this.ObjectAliases.Add(objectName, container);//GetObject(aliasName)时可返回
				return this;
			}

			public IObjectNamespace AddObject(Type objectType, IObjectContainer container) {
				if(objectType == null) throw new ArgumentNullException(nameof(objectType));
				if(container == null) throw new ArgumentNullException(nameof(container));

				var typedObjects = this.TypedObjects;
				if(!typedObjects.ContainsKey(objectType)) {
					typedObjects.Add(objectType, new TypedObject());
				}
				typedObjects[objectType].Add(container);//GetObject<>时可返回
				return this;
			}

			public bool RemoveObject(string objectName) {
				if(string.IsNullOrEmpty(objectName)) throw new ArgumentNullException(nameof(objectName));
				return this.ObjectAliases.Remove(objectName);
			}

			public bool RemoveObject(Type objectType) {
				if(objectType == null) throw new ArgumentNullException(nameof(objectType));
				return this.TypedObjects.Remove(objectType);
			}
		}

		internal class TypedObject : ITypedObjectContainer
		{
			public IObjectContainer Container { get; private set; }
			public IList<IObjectContainer> Containers { get; private set; }

			public ITypedObjectContainer Add(IObjectContainer container) {
				this.Container = container;//最后一个被加入的为准
				if(this.Containers == null) {
					this.Containers = new List<IObjectContainer>();
				}
				if (!this.Containers.Contains(container)) {
					this.Containers.Add(container);
				}
				return this;
			}
		}

		public virtual IObjectNamespace GetObjectNamespace() {
			return this.globalObjectNamespace;
		}
	}
}

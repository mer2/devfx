/* Copyright(c) 2005-2011 R2@DevFx.NET, License(LGPL) */

using System.Collections;
using DevFx.Core.Lifetime;

namespace DevFx.Core
{
	public abstract class LifetimePolicy : ObjectContextBase, ILifetimePolicy
	{
		protected LifetimePolicy() : base(null) { }

		protected abstract object GetObjectInternal(IDictionary items);
		protected abstract void SetObjectInternal(object newValue, IDictionary items);
		protected abstract void RemoveObjectInternal(IDictionary items);

		#region ILifetimePolicy Members

		object ILifetimePolicy.GetObject(IDictionary items) {
			return this.GetObjectInternal(items);
		}

		void ILifetimePolicy.SetObject(object newValue, IDictionary items) {
			this.SetObjectInternal(newValue, items);
		}

		void ILifetimePolicy.RemoveObject(IDictionary items) {
			this.RemoveObjectInternal(items);
		}

		#endregion
	}

	public abstract class ObjectContainer : LifetimePolicy, IObjectContainer
	{
		protected virtual void Init(IObjectDescription objectDescription, IObjectBuilder objectBuilder) {
			this.ObjectDescription = objectDescription;
			this.ObjectBuilder = objectBuilder;
		}

		protected virtual void InitCompleted() {
		}

		protected virtual object CreateObject(IDictionary items) {
			return this.ObjectBuilder.CreateObject(this.ObjectDescription, items);
		}

		protected virtual IObjectDescription ObjectDescription { get; private set; }
		protected virtual IObjectBuilder ObjectBuilder { get; private set; }

		public static IObjectContainer CreateDefault() {
			return new SingletonObjectContainer();
		}

		#region ILifetimeContainer Members

		void IObjectContainer.Init(IObjectDescription objectDescription, IObjectBuilder objectBuilder) {
			this.Init(objectDescription, objectBuilder);
		}

		void IObjectContainer.InitCompleted() {
			this.InitCompleted();
		}

		IObjectDescription IObjectContainer.ObjectDescription => this.ObjectDescription;
		IObjectBuilder IObjectContainer.ObjectBuilder => this.ObjectBuilder;

		#endregion
	}
}

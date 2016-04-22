/* Copyright(c) 2005-2011 R2@DevFx.NET, License(LGPL) */

using System.Collections;
using HTB.DevFx.Core.Config;
using HTB.DevFx.Core.Lifetime;

namespace HTB.DevFx.Core
{
	public abstract class LifetimeContainer : ILifetimeContainer
	{
		protected virtual void Init(IObjectSetting objectSetting, IObjectBuilder objectBuilder) {
			this.ObjectSetting = objectSetting;
			this.ObjectBuilder = objectBuilder;
		}

		protected virtual void InitCompleted() {
		}

		protected virtual object GetObject(IDictionary items) {
			return this.GetObjectInternal(items);
		}

		protected virtual void InitObject(object instance) {
			if (instance != null && instance is IInitializable) {
				((IInitializable)instance).Init();
			}
		}

		protected virtual IObjectSetting ObjectSetting { get; private set; }
		protected virtual IObjectBuilder ObjectBuilder { get; private set; }

		protected abstract object GetObjectInternal(IDictionary items);
		protected abstract void SetObjectInternal(object newValue, IDictionary items);
		protected abstract void RemoveObjectInternal(IDictionary items);

		public static ILifetimeContainer CreateDefault() {
			return new TransientLifetimeContainer();
		}

		#region ILifetimeContainer Members

		void ILifetimeContainer.Init(IObjectSetting objectSetting, IObjectBuilder objectBuilder) {
			this.Init(objectSetting, objectBuilder);
		}

		void ILifetimeContainer.InitCompleted() {
			this.InitCompleted();
		}

		IObjectSetting ILifetimeContainer.ObjectSetting {
			get { return this.ObjectSetting; }
		}

		IObjectBuilder ILifetimeContainer.ObjectBuilder {
			get { return this.ObjectBuilder; }
		}

		#endregion

		#region ILifetimePolicy Members

		object ILifetimePolicy.GetObject() {
			return this.GetObject(null);
		}

		void ILifetimePolicy.SetObject(object newValue) {
			this.SetObjectInternal(newValue, null);
		}

		void ILifetimePolicy.RemoveObject() {
			this.RemoveObjectInternal(null);
		}

		object ILifetimePolicy.GetObject(IDictionary items) {
			return this.GetObject(items);
		}

		void ILifetimePolicy.SetObject(object newValue, IDictionary items) {
			this.SetObjectInternal(newValue, items);
		}

		void ILifetimePolicy.RemoveObject(IDictionary items) {
			this.RemoveObjectInternal(items);
		}

		#endregion
	}
}

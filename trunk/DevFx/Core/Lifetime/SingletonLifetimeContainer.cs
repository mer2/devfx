/* Copyright(c) 2005-2011 R2@DevFx.NET, License(LGPL) */

using System.Collections;

namespace HTB.DevFx.Core.Lifetime
{
	public class SingletonLifetimeContainer : LifetimeContainer
	{
		private object instance;
		protected override object GetObjectInternal(IDictionary items) {
			if(this.instance == null) {
				lock(this) {
					if(this.instance == null) {
						this.instance = this.ObjectBuilder.CreateObject(this.ObjectSetting, items);
					}
				}
			}
			return this.instance;
		}

		protected override void SetObjectInternal(object newValue, IDictionary items) {
			this.instance = newValue;
		}

		protected override void RemoveObjectInternal(IDictionary items) {
			this.instance = null;
		}
	}
}
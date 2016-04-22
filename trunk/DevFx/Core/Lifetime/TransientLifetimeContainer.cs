/* Copyright(c) 2005-2011 R2@DevFx.NET, License(LGPL) */

using System.Collections;

namespace HTB.DevFx.Core.Lifetime
{
	public class TransientLifetimeContainer : LifetimeContainer
	{
		protected override object GetObjectInternal(IDictionary items) {
			return this.ObjectBuilder.CreateObject(this.ObjectSetting, items);
		}

		protected override void SetObjectInternal(object newValue, IDictionary items) {
		}

		protected override void RemoveObjectInternal(IDictionary items) {
		}
	}
}
/* Copyright(c) 2005-2011 R2@DevFx.NET, License(LGPL) */

using System.Collections;

namespace DevFx.Core.Lifetime
{
	[TypeAliase("Transient")]
	public class TransientObjectContainer : ObjectContainer
	{
		protected override object GetObjectInternal(IDictionary items) {
			return this.CreateObject(items);
		}

		protected override void SetObjectInternal(object newValue, IDictionary items) {
		}

		protected override void RemoveObjectInternal(IDictionary items) {
		}
	}
}
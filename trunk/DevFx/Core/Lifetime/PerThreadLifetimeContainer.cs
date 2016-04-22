/* Copyright(c) 2005-2011 R2@DevFx.NET, License(LGPL) */

using System;
using System.Collections;
using System.Collections.Generic;

namespace HTB.DevFx.Core.Lifetime
{
	public class PerThreadLifetimeContainer : LifetimeContainer
	{
		[ThreadStatic]
		private static Dictionary<Guid, object> values;
		private readonly Guid key;

		public PerThreadLifetimeContainer() {
			this.key = Guid.NewGuid();
		}

		protected override object GetObjectInternal(IDictionary items) {
			EnsureValues();

			object value;
			values.TryGetValue(this.key, out value);
			if (value == null) {
				lock (this) {
					value = this.ObjectBuilder.CreateObject(this.ObjectSetting, items);
					values.Add(this.key, value);
				}
			}
			return value;
		}

		protected override void SetObjectInternal(object newValue, IDictionary items) {
			EnsureValues();

			lock (this) {
				values[this.key] = newValue;
			}
		}

		protected override void RemoveObjectInternal(IDictionary items) {
			EnsureValues();

			lock (this) {
				values.Remove(this.key);
			}
		}
		
		private static void EnsureValues() {
			// no need for locking, values is TLS
			if (values == null) {
				values = new Dictionary<Guid, object>();
			}
		}
	}
}

/* Copyright(c) 2005-2011 R2@DevFx.NET, License(LGPL) */

using System;
using System.Collections;
using System.Collections.Generic;

namespace DevFx.Core.Lifetime
{
	[TypeAliase("PerThread")]
	public class PerThreadObjectContainer : ObjectContainer
	{
		[ThreadStatic]
		private static Dictionary<Guid, object> values;
		private readonly Guid key;

		public PerThreadObjectContainer() {
			this.key = Guid.NewGuid();
		}

		protected override object GetObjectInternal(IDictionary items) {
			EnsureValues();

			values.TryGetValue(this.key, out var value);
			if (value == null) {
				lock (this) {
					value = this.CreateObject(items);
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

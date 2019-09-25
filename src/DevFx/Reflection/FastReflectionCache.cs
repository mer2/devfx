/* Copyright(c) 2005-2011 R2@DevFx.NET, License(LGPL) */

using System.Collections.Generic;

namespace DevFx.Reflection
{
	internal abstract class FastReflectionCache<TKey, TValue> : IFastReflectionCache<TKey, TValue> where TKey : class
	{
		private readonly Dictionary<TKey, TValue> cache = new Dictionary<TKey, TValue>();

		public TValue Get(TKey key) {
			if(!this.cache.TryGetValue(key, out var value)) {
				lock(key) {
					if(!this.cache.TryGetValue(key, out value)) {
						value = this.Create(key);
						this.cache[key] = value;
					}
				}
			}
			return value;
		}

		protected abstract TValue Create(TKey key);
	}
}
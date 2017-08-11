namespace HTB.DevFx.Cache
{
	public static class CacheHelper
	{
		public static T GetObjectFromCache<T>(this ICache cache, string key, Func<T> missingHandler, ICacheDependency dependency = null) where T : class {
			key = key.ToLowerInvariant();
			lock (key) {
				var value = cache[key] as T;
				if(value == null) {
					value = missingHandler();
					if(value != null) {
						if(dependency == null) {
							cache[key] = value;
						} else {
							cache[key, dependency] = value;
						}
					}
				}
				return value;
			}
		}

		public static T GetValueFromCache<T>(this ICache cache, string key, Func<T> missingHandler, ICacheDependency dependency = null) where T : struct {
			key = key.ToLowerInvariant();
			lock (key) {
				var cachedValue = cache[key];
				if(!(cachedValue is T)) {
					var value = missingHandler();
					if(dependency == null) {
						cache[key] = value;
					} else {
						cache[key, dependency] = value;
					}
					return value;
				}
				return (T)cachedValue;
			}
		}
	}
}

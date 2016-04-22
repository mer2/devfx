/* Copyright(c) 2005-2011 R2@DevFx.NET, License(LGPL) */

using HTB.DevFx.Cache;
using HTB.DevFx.Config;
using HTB.DevFx.Data.Config;
using HTB.DevFx.Data.Entities;

namespace HTB.DevFx.Data
{
	public class ResultCacheModule : ResultModuleBase
	{
		protected virtual ICacheService CacheService {
			get { return Cache.CacheService.Current; }
		}

		protected virtual string GetCacheKey(IDataCacheSetting setting, IDbExecuteContext ctx) {
			var cacheKey = setting.CacheKey;
			if (string.IsNullOrEmpty(cacheKey)) {
				cacheKey = ctx.Statement.Name;
			}
			var key = cacheKey + ":";
			var parameters = setting.Parameters;
			if (string.IsNullOrEmpty(parameters)) {
				key += ctx.Parameters.ToString(null);
			} else {
				key += ctx.Parameters.ToString(parameters.Split(','), null);
			}
			return key;
		}

		protected override void OnResultExecuteInternal(IDbResultContext ctx) {
			var settting = ctx.ExecuteContext.Statement.ConfigSetting.ToCachedSetting<DataCacheSetting>("cache");
			if (settting != null) {
				if (settting.Cacheable) {
					this.CacheExecuteResult(settting, ctx);
				}
			}
		}

		protected virtual void CacheExecuteResult(IDataCacheSetting setting, IDbResultContext ctx) {
			var executeContext = ctx.ExecuteContext;
			var key = this.GetCacheKey(setting, executeContext);
			var cache = this.CacheService.GetCache(setting.CacheName);
			object result;
			if (setting.CacheAction == CacheAction.Remove) {
				result = ctx.ResultHandler.ExecuteResult(executeContext, ctx.ResultType, ctx.ResultInstance);
				cache.Remove(key);
			} else {
				if(!cache.TryGet(key, out result)) {
					lock(key) {
						if(!cache.TryGet(key, out result)) {
							result = ctx.ResultHandler.ExecuteResult(executeContext, ctx.ResultType, ctx.ResultInstance);
							var dependencyName = setting.DependencyName;
							ICacheDependency dependency = null;
							if (!string.IsNullOrEmpty(dependencyName)) {
								dependency = ObjectService.Current.GetOrCreateObject<ICacheDependency>(dependencyName);
							}
							if (dependency != null) {
								cache.Add(key, result, dependency);
							} else {
								cache.Add(key, result);
							}
						}
					}
				}
			}
			ctx.ResultInstance = result;
			ctx.ResultHandled = true;
		}
	}
}

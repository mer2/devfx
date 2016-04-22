/* Copyright(c) 2005-2011 R2@DevFx.NET, License(LGPL) */

namespace HTB.DevFx.Cache
{
	/// <summary>
	/// 缓存服务接口
	/// </summary>
	public interface ICacheService
	{
		/// <summary>
		/// 获取已配置的缓存器
		/// </summary>
		/// <param name="cacheName">配置文件中配置的缓存器名称</param>
		/// <returns>ICache的实例</returns>
		ICache GetCache(string cacheName);

		/// <summary>
		/// 获取缓存项值
		/// </summary>
		/// <param name="cacheName">配置文件中配置的缓存器名称</param>
		/// <param name="key">缓存项健值</param>
		/// <returns>缓存项值，如果没有命中，则返回<c>null</c></returns>
		object GetCacheValue(string cacheName, string key);

		/// <summary>
		/// 获取缓存项值
		/// </summary>
		/// <param name="cacheName">配置文件中配置的缓存器名称</param>
		/// <param name="key">缓存项健值</param>
		/// <param name="throwOnError">如果有错误，是否抛出异常</param>
		/// <returns>缓存项值，如果没有命中，则返回<c>null</c></returns>
		object GetCacheValue(string cacheName, string key, bool throwOnError);
	}
}

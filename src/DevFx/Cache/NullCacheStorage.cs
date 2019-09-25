/* Copyright(c) 2005-2011 R2@DevFx.NET, License(LGPL) */

using DevFx.Utils;

namespace DevFx.Cache
{
	/// <summary>
	/// 内存数据存储方式的存储器
	/// </summary>
	public class NullCacheStorage : CollectionBase<ICacheItem>, ICacheStorage
	{
		#region Implementation of ICacheStorage

		/// <summary>
		/// 获取存储项
		/// </summary>
		/// <param name="key">存储项的健值</param>
		/// <returns>存储的对象，如果存储中没有命中，则返回<c>null</c></returns>
		public ICacheItem Get(string key) {
			return (ICacheItem)this.BaseGet(key);
		}

		/// <summary>
		/// 获取存储项
		/// </summary>
		/// <param name="index">存储项的索引值</param>
		/// <returns>存储的对象，如果存储中没有命中，则返回<c>null</c></returns>
		public ICacheItem Get(int index) {
			return (ICacheItem)this.BaseGet(index);
		}

		/// <summary>
		/// 设置存储项
		/// </summary>
		/// <param name="index">存储项的索引值</param>
		/// <param name="value">存储的对象</param>
		/// <remarks>
		/// 仅针对存在存储项，若不存在，则不进行任何操作
		/// </remarks>
		public void Set(int index, ICacheItem value) {
			this.BaseSet(index, value);
		}

		/// <summary>
		/// 获得此存储器中所有项的健值
		/// </summary>
		/// <returns>健值列表（数组）</returns>
		public string[] GetAllKeys() {
			return this.BaseGetAllKeys();
		}

		/// <summary>
		/// 获取此存储器中所有项的值
		/// </summary>
		/// <returns>存储项列表（数组）</returns>
		public ICacheItem[] GetAllValues() {
			return (ICacheItem[])this.BaseGetAllValues();
		}

		/// <summary>
		/// 创建存储项实体
		/// </summary>
		/// <param name="key">健值</param>
		/// <param name="value">实际缓存的对象</param>
		/// <param name="cacheDependency">过期依赖</param>
		/// <returns>存储项实体</returns>
		public ICacheItem CreateCacheItem(string key, object value, ICacheDependency cacheDependency) {
			return new CacheItem(key, value, cacheDependency);
		}

		#endregion
	}
}

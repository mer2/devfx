/* Copyright(c) 2005-2011 R2@DevFx.NET, License(LGPL) */

namespace DevFx.Cache
{
	/// <summary>
	/// 缓存器的存储接口
	/// </summary>
	public interface ICacheStorage
	{
		/// <summary>
		/// 获取此存储器所存储项的个数
		/// </summary>
		int Count { get; }

		/// <summary>
		/// 添加一项到存储器中
		/// </summary>
		/// <param name="key">存储项的健值</param>
		/// <param name="value">存储的对象</param>
		/// <remarks>
		/// 如果存在相同的健值，则更新存储的对象
		/// </remarks>
		void Add(string key, ICacheItem value);

		/// <summary>
		/// 获取存储项
		/// </summary>
		/// <param name="key">存储项的健值</param>
		/// <returns>存储的对象，如果存储中没有命中，则返回<c>null</c></returns>
		ICacheItem Get(string key);

		/// <summary>
		/// 获取存储项
		/// </summary>
		/// <param name="index">存储项的索引值</param>
		/// <returns>存储的对象，如果存储中没有命中，则返回<c>null</c></returns>
		ICacheItem Get(int index);

		/// <summary>
		/// 设置存储项
		/// </summary>
		/// <param name="key">存储项的健值</param>
		/// <param name="value">存储的对象</param>
		/// <remarks>
		/// 仅针对存在存储项，若不存在，则不进行任何操作
		/// </remarks>
		void Set(string key, ICacheItem value);

		/// <summary>
		/// 设置存储项
		/// </summary>
		/// <param name="index">存储项的索引值</param>
		/// <param name="value">存储的对象</param>
		/// <remarks>
		/// 仅针对存在存储项，若不存在，则不进行任何操作
		/// </remarks>
		void Set(int index, ICacheItem value);

		/// <summary>
		/// 移除存储项
		/// </summary>
		/// <param name="key">存储项的健值</param>
		void Remove(string key);

		/// <summary>
		/// 在指定的位置移除存储项
		/// </summary>
		/// <param name="index">存储项的索引值</param>
		void RemoveAt(int index);

		/// <summary>
		/// 判断存储器中是否包含指定健值的存储项
		/// </summary>
		/// <param name="key">存储项的健值</param>
		/// <returns>是/否</returns>
		bool Contains(string key);

		/// <summary>
		/// 清除此存储器中所有的项
		/// </summary>
		void Clear();

		/// <summary>
		/// 获得此存储器中所有项的健值
		/// </summary>
		/// <returns>健值列表（数组）</returns>
		string[] GetAllKeys();

		/// <summary>
		/// 获取此存储器中所有项的值
		/// </summary>
		/// <returns>存储项列表（数组）</returns>
		ICacheItem[] GetAllValues();

		/// <summary>
		/// 创建存储项实体
		/// </summary>
		/// <param name="key">健值</param>
		/// <param name="value">实际缓存的对象</param>
		/// <param name="cacheDependency">过期依赖</param>
		/// <returns>存储项实体</returns>
		ICacheItem CreateCacheItem(string key, object value, ICacheDependency cacheDependency);
	}
}

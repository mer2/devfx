/* Copyright(c) 2005-2011 R2@DevFx.NET, License(LGPL) */

using System;
using System.Collections.Generic;

namespace HTB.DevFx.Cache.CacheStorages
{
	/// <summary>
	/// 存储器远端（服务器）类
	/// </summary>
	/// <remarks>
	/// 配合 <see cref="RemoteCacheStorageProxy"/>，把存储数据存储到远端<br />
	/// 注意：应该把本类配置为Singleton模式
	/// </remarks>
	public class RemoteCacheStorageServer : MarshalByRefObject, ICacheStorage
	{
		#region protected members

		protected static readonly List<RemoteCacheStorageServer> instances = new List<RemoteCacheStorageServer>();

		private ICacheStorage cacheStorage;
		protected virtual ICacheStorage CacheStorage {
			get { return this.cacheStorage; }
			set { this.cacheStorage = value; }
		}

		#endregion

		#region constructors

		protected RemoteCacheStorageServer() {
			instances.Add(this);
			this.cacheStorage = new NullCacheStorage();
		}

		#endregion

		#region public static members

		/// <summary>
		/// 获取本类的实例列表
		/// </summary>
		/// <remarks>
		/// 利用此属性，可以把客户端轮询过期时间设置为-1，然后在服务器端进行轮询，以提高效率
		/// </remarks>
		public static RemoteCacheStorageServer[] Instances {
			get {
				return instances.ToArray();
			}
		}

		#endregion

		#region ICacheStorage Members

		/// <summary>
		/// 获取此存储器所存储项的个数
		/// </summary>
		int ICacheStorage.Count {
			get { return this.CacheStorage.Count; }
		}

		/// <summary>
		/// 添加一项到存储器中
		/// </summary>
		/// <param name="key">存储项的健值</param>
		/// <param name="value">存储的对象</param>
		/// <remarks>
		/// 如果存在相同的健值，则更新存储的对象
		/// </remarks>
		void ICacheStorage.Add(string key, ICacheItem value) {
			this.CacheStorage.Add(key, value);
		}

		/// <summary>
		/// 获取存储项
		/// </summary>
		/// <param name="key">存储项的健值</param>
		/// <returns>存储的对象，如果存储中没有命中，则返回<c>null</c></returns>
		ICacheItem ICacheStorage.Get(string key) {
			return this.CacheStorage.Get(key);
		}

		/// <summary>
		/// 获取存储项
		/// </summary>
		/// <param name="index">存储项的索引值</param>
		/// <returns>存储的对象，如果存储中没有命中，则返回<c>null</c></returns>
		ICacheItem ICacheStorage.Get(int index) {
			return this.CacheStorage.Get(index);
		}

		/// <summary>
		/// 设置存储项
		/// </summary>
		/// <param name="key">存储项的健值</param>
		/// <param name="value">存储的对象</param>
		/// <remarks>
		/// 仅针对存在存储项，若不存在，则不进行任何操作
		/// </remarks>
		void ICacheStorage.Set(string key, ICacheItem value) {
			this.CacheStorage.Set(key, value);
		}

		/// <summary>
		/// 设置存储项
		/// </summary>
		/// <param name="index">存储项的索引值</param>
		/// <param name="value">存储的对象</param>
		/// <remarks>
		/// 仅针对存在存储项，若不存在，则不进行任何操作
		/// </remarks>
		void ICacheStorage.Set(int index, ICacheItem value) {
			this.CacheStorage.Set(index, value);
		}

		/// <summary>
		/// 移除存储项
		/// </summary>
		/// <param name="key">存储项的健值</param>
		void ICacheStorage.Remove(string key) {
			this.CacheStorage.Remove(key);
		}

		/// <summary>
		/// 在指定的位置移除存储项
		/// </summary>
		/// <param name="index">存储项的索引值</param>
		void ICacheStorage.RemoveAt(int index) {
			this.CacheStorage.RemoveAt(index);
		}

		/// <summary>
		/// 判断存储器中是否包含指定健值的存储项
		/// </summary>
		/// <param name="key">存储项的健值</param>
		/// <returns>是/否</returns>
		bool ICacheStorage.Contains(string key) {
			return this.CacheStorage.Contains(key);
		}

		/// <summary>
		/// 清除此存储器中所有的项
		/// </summary>
		void ICacheStorage.Clear() {
			this.CacheStorage.Clear();
		}

		/// <summary>
		/// 获得此存储器中所有项的健值
		/// </summary>
		/// <returns>健值列表（数组）</returns>
		string[] ICacheStorage.GetAllKeys() {
			return this.CacheStorage.GetAllKeys();
		}

		/// <summary>
		/// 获取此存储器中所有项的值
		/// </summary>
		/// <returns>存储项列表（数组）</returns>
		ICacheItem[] ICacheStorage.GetAllValues() {
			return this.CacheStorage.GetAllValues();
		}

		/// <summary>
		/// 创建存储项实体
		/// </summary>
		/// <param name="key">健值</param>
		/// <param name="value">实际缓存的对象</param>
		/// <param name="cacheDependency">过期依赖</param>
		/// <returns>存储项实体</returns>
		ICacheItem ICacheStorage.CreateCacheItem(string key, object value, ICacheDependency cacheDependency) {
			return this.CacheStorage.CreateCacheItem(key, value, cacheDependency);
		}

		#endregion
	}
}

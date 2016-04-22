/* Copyright(c) 2005-2011 R2@DevFx.NET, License(LGPL) */

using System;
using System.Collections.Generic;
using System.Collections.Specialized;

namespace HTB.DevFx.Utils
{
	/// <summary>
	/// 集合基础类（泛型）
	/// </summary>
	/// <typeparam name="T">集合收集对象的类型</typeparam>
	public class CollectionBase<T> : NameObjectCollectionBase
	{
		/// <summary>
		/// 构造函数
		/// </summary>
		public CollectionBase() : this(false) {}

		/// <summary>
		/// 构造函数
		/// </summary>
		/// <param name="uniqueKey">键是否限制为唯一</param>
		public CollectionBase(bool uniqueKey) {
			this.uniqueKey = uniqueKey;
		}

		private bool uniqueKey;

		/// <summary>
		/// 键是否限制为唯一
		/// </summary>
		public virtual bool UniqueKey {
			get { return this.uniqueKey; }
			protected set { this.uniqueKey = value; }
		}

		/// <summary>
		/// 按索引的方式获取项
		/// </summary>
		/// <param name="index">索引</param>
		public virtual T this[int index] {
			get { return (T)this.BaseGet(index); }
		}

		/// <summary>
		/// 按键值方式获取项
		/// </summary>
		/// <param name="key">键值</param>
		public virtual T this[string key] {
			get { return (T)this.BaseGet(key); }
		}

		/// <summary>
		/// 添加一项到集合中
		/// </summary>
		/// <param name="key">键值</param>
		/// <param name="value">项</param>
		public virtual void Add(string key, T @value) {
			this.Add(key, value, false);
		}

		/// <summary>
		/// 添加一项到集合中
		/// </summary>
		/// <param name="key">键值</param>
		/// <param name="value">项</param>
		/// <param name="ignoreExists">如果存在则忽略</param>
		public virtual void Add(string key, T @value, bool ignoreExists) {
			if (this.UniqueKey && this.Contains(key)) {
				if(ignoreExists) {
					return;
				}
				throw new Exception("已存在键值：" + key);
			}
			this.BaseAdd(key, @value);
		}

		/// <summary>
		/// 添加/替换一项到集合中
		/// </summary>
		/// <param name="key">键值</param>
		/// <param name="value">项</param>
		public virtual void Set(string key, T @value) {
			this.BaseSet(key, @value);
		}

		/// <summary>
		/// 集合中是否包含某项
		/// </summary>
		/// <param name="key">键值</param>
		/// <returns>是/否</returns>
		public virtual bool Contains(string key) {
			return (this.BaseGet(key) != null);
		}

		/// <summary>
		/// 移除某项
		/// </summary>
		/// <param name="key">键值</param>
		public virtual void Remove(string key) {
			this.BaseRemove(key);
		}

		/// <summary>
		/// 在指定处移除某项
		/// </summary>
		/// <param name="index">索引</param>
		public virtual void RemoveAt(int index) {
			this.BaseRemoveAt(index);
		}

		/// <summary>
		/// 清空集合中所有元素
		/// </summary>
		public virtual void Clear() {
			this.BaseClear();
		}

		/// <summary>
		/// 复制到数组中
		/// </summary>
		/// <returns>数组</returns>
		public virtual T[] CopyToArray() {
			return this.BaseGetAllValues(typeof(T)) as T[];
		}

		/// <summary>
		/// 获取具有相同Key的项
		/// </summary>
		/// <param name="key">键值</param>
		/// <returns>具有相同Key的项</returns>
		public virtual T[] GetItem(string key) {
			var list = new List<T>();
			for(var i = 0; i < this.Count; i++) {
				if(string.Compare(this.Keys[i], key, true) == 0) {
					list.Add(this[i]);
				}
			}
			return list.ToArray();
		}

		/// <summary>
		/// 获取所有值
		/// </summary>
		public virtual T[] Values {
			get { return this.BaseGetAllValues(typeof(T)) as T[]; }
		}
	}
}
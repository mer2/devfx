/* Copyright(c) 2005-2011 R2@DevFx.NET, License(LGPL) */

using System.Collections;
using System.Collections.Specialized;

namespace HTB.DevFx.Core
{
	/// <summary>
	/// 对象上下文基类
	/// </summary>
	public abstract class ObjectContextBase : IObjectContext
	{
		/// <summary>
		/// 构造
		/// </summary>
		/// <param name="items">传递的上下文</param>
		protected ObjectContextBase(IDictionary items) {
			this.items = items;
		}

		/// <summary>
		/// 获取一个上下文值
		/// </summary>
		/// <param name="key">键</param>
		/// <returns>上下文值</returns>
		protected virtual object GetItem(object key) {
			return this.Items[key];
		}

		/// <summary>
		/// 设置一个上下文值
		/// </summary>
		/// <param name="key">键</param>
		/// <param name="value">上下文值</param>
		/// <returns>上下文值</returns>
		protected virtual object SetItem(object key, object value) {
			this.Items[key] = value;
			return value;
		}

		/// <summary>
		/// 获取一个上下文值（键为类型）
		/// </summary>
		/// <typeparam name="T">值的类型</typeparam>
		/// <returns>上下文值</returns>
		protected T GetItem<T>() {
			return (T)this.GetItem(typeof(T));
		}

		/// <summary>
		/// 设置一个上下文值（键为类型）
		/// </summary>
		/// <typeparam name="T">值的类型</typeparam>
		/// <param name="value">值实例</param>
		/// <returns>上下文值</returns>
		protected T SetItem<T>(T value) {
			return (T)this.SetItem(typeof(T), value);
		}

		private IDictionary items;
		/// <summary>
		/// 传递的上下文
		/// </summary>
		public virtual IDictionary Items {
			get { return this.items ?? (this.items = new HybridDictionary()); }
		}
	}
}

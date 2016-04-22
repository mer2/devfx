/* Copyright(c) 2005-2011 R2@DevFx.NET, License(LGPL) */

using System.Configuration;

namespace HTB.DevFx.Config.DotNetConfig
{
	/// <summary>
	/// 配置集合（泛型）
	/// </summary>
	/// <typeparam name="T">集合元素类型</typeparam>
	public class ConfigCollection<T> : ConfigurationElementCollectionBase where T : ConfigurationElement, new()
	{
		/// <summary>
		/// 按索引方式获取元素
		/// </summary>
		/// <param name="index">索引</param>
		/// <returns>元素</returns>
		public virtual T this[int index] {
			get { return (T)this.BaseGet(index); }
		}

		/// <summary>
		/// 按键值方式获取元素
		/// </summary>
		/// <param name="key">键值</param>
		/// <returns>元素</returns>
		public virtual T this[object key] {
			get { return (T)this.BaseGet(key); }
		}

		/// <summary>
		/// 集合转换成数组
		/// </summary>
		/// <returns>元素数组</returns>
		public virtual T[] ToArray() {
			var values = new T[this.Count];
			this.CopyTo(values, 0);
			return values;
		}

		/// <summary>
		/// 创建新元素
		/// </summary>
		/// <returns>新元素</returns>
		protected override ConfigurationElement CreateNewElement() {
			return new T();
		}

		/// <summary>
		/// 获得元素的键
		/// </summary>
		/// <param name="element">配置元素</param>
		/// <returns>键</returns>
		protected override object GetElementKey(ConfigurationElement element) {
			return this.GetElementKey((T)element);
		}

		/// <summary>
		/// 获得元素的键
		/// </summary>
		/// <param name="element">配置元素</param>
		/// <returns>键</returns>
		protected virtual object GetElementKey(T element) {
			return base.GetElementKey(element);
		}
	}
}
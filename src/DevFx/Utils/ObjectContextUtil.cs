/* Copyright(c) 2005-2011 R2@DevFx.NET, License(LGPL) */

using System;
using System.Collections;
using System.Collections.Specialized;
using System.Collections.Generic;

namespace DevFx.Utils
{
	/// <summary>
	/// 对象附属上下文扩展工具
	/// </summary>
	public static class ObjectContextUtil
	{
		/// <summary>
		/// 获取附属在对象的上下文
		/// </summary>
		/// <param name="instance">对象</param>
		/// <returns>上下文</returns>
		public static IDictionary GetObjectContext(this object instance) {
			if(instance == null) {
				return null;
			}
			var type = instance.GetType();
			var hashCode = instance.GetHashCode();
			ContextWrap context = null;
			IDictionary<int, ContextWrap> typeItems;
			lock (objectCaches) {
				if (!objectCaches.TryGetValue(type, out typeItems)) {
					typeItems = new Dictionary<int, ContextWrap>();
					objectCaches.Add(type, typeItems);
					context = new ContextWrap { HashCode = hashCode, Items = new HybridDictionary(), Reference = new WeakReference(instance) };
					typeItems.Add(hashCode, context);
				}
			}
			lock (typeItems) {
				if(context == null && !typeItems.TryGetValue(hashCode, out context)) {
					context = new ContextWrap { HashCode = hashCode, Items = new HybridDictionary(), Reference = new WeakReference(instance) };
					typeItems.Add(hashCode, context);
				}
			}
			return context.Items;
		}

		/// <summary>
		/// 获取附属在对象的上下文项
		/// </summary>
		/// <param name="instance">对象</param>
		/// <param name="key">项键值</param>
		/// <returns>上下文项</returns>
		public static object GetObjectContext(this object instance, object key) {
			var items = instance.GetObjectContext();
			return items?[key ?? instance];
		}

		/// <summary>
		/// 获取附属在对象的上下文项
		/// </summary>
		/// <param name="instance">对象</param>
		/// <param name="key">项键值</param>
		/// <returns>上下文项</returns>
		public static T GetObjectContext<T>(this object instance, object key) {
			var items = instance.GetObjectContext();
			return (T) items?[key ?? instance];
		}

		/// <summary>
		/// 获取附属在对象的上下文项
		/// </summary>
		/// <param name="instance">对象</param>
		/// <param name="key">项键值</param>
		/// <param name="missingHandler">上下文项不存在处理例程</param>
		/// <returns>上下文项</returns>
		public static object GetObjectContext(this object instance, object key, Func<object> missingHandler) {
			key = key ?? instance;
			var items = instance.GetObjectContext();
			object value = null;
			if(items != null) {
				if(!items.Contains(key) && missingHandler != null) {
					lock (items) {
						if(!items.Contains(key)) {
							value = missingHandler();
							items.Add(key, value);
						}
					}
				}
				value = items[key];
			}
			return value;
		}

		/// <summary>
		/// 获取附属在对象的上下文项
		/// </summary>
		/// <param name="instance">对象</param>
		/// <param name="key">项键值</param>
		/// <param name="missingHandler">上下文项不存在处理例程</param>
		/// <returns>上下文项</returns>
		public static T GetObjectContext<T>(this object instance, object key, Func<T> missingHandler) {
			return (T)instance.GetObjectContext(key, missingHandler != null ? () => (object)missingHandler() : (Func<object>)null);
		}

		private static readonly IDictionary<Type, IDictionary<int, ContextWrap>> objectCaches = new Dictionary<Type, IDictionary<int, ContextWrap>>();
		private class ContextWrap
		{
			public int HashCode;
			public IDictionary Items;
			public WeakReference Reference;
		}
	}
}
/* Copyright(c) 2005-2011 R2@DevFx.NET, License(LGPL) */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using HTB.DevFx.Exceptions;
using HTB.DevFx.Reflection;

namespace HTB.DevFx.Utils
{
	public abstract class ReflectionHelper : TypeHelper
	{
		internal ReflectionHelper() { }
		static ReflectionHelper() {
			lock(ConstructorInvokerLockObject) {
				ConstructorInvoker = ConstructorInvoke;
			}
			lock(MethodInvokerLockObject) {
				MethodInvoker = MethodInvoke;
			}
		}
		static object ConstructorInvoke(ConstructorInfo ctor, object[] parameterValues) {
			return ctor.FastInvoke(parameterValues);
		}
		static object MethodInvoke(MethodInfo method, object instance, object[] parameterValues) {
			return method.FastInvoke(instance, parameterValues);
		}

		/// <summary>
		/// 通过反射获取属性/字段值
		/// </summary>
		/// <param name="obj">类型实例</param>
		/// <param name="propertyName">属性名</param>
		/// <returns>属性值</returns>
		public static object GetPropertyValue(object obj, string propertyName) {
			if (obj == null) {
				return null;
			}
			Type type;
			var flags = DefaultFieldBindingFlags;
			if (obj is Type) {
				type = (Type)obj;
				flags |= BindingFlags.Static;
			} else {
				type = obj.GetType();
			}
			var property = type.GetProperty(propertyName, flags);
			if (property != null && property.CanRead) {
				return property.FastGetValue(obj);
			}
			var field = type.GetField(propertyName, flags);
			return field != null ? field.FastGetValue(obj) : null;
		}

		/// <summary>
		/// 通过反射设置属性/字段值
		/// </summary>
		/// <param name="obj">类型实例</param>
		/// <param name="propertyName">属性名</param>
		/// <param name="newValue">属性值</param>
		public new static void SetPropertyValue(object obj, string propertyName, object newValue) {
			var value = newValue;
			SetPropertyValue(obj, propertyName, (x, p) => value);
		}

		/// <summary>
		/// 通过反射设置属性/字段值
		/// </summary>
		/// <param name="obj">类型实例</param>
		/// <param name="propertyName">属性名</param>
		/// <param name="valueGetter">属性值获取器</param>
		public static void SetPropertyValue(object obj, string propertyName, Func<object, IFieldOrPropertyInfo, object> valueGetter) {
			if (obj == null) {
				return;
			}
			SetPropertyValueInternal(obj, propertyName, valueGetter);
		}

		private static void SetPropertyValueInternal(object obj, string propertyName, Func<object, IFieldOrPropertyInfo, object> valueGetter) {
			Type type;
			var flags = DefaultFieldBindingFlags;
			if (obj is Type) {
				type = (Type)obj;
				flags |= BindingFlags.Static;
			} else {
				type = obj.GetType();
			}
			object value = null;
			try {
				var property = type.GetProperty(propertyName, flags);
				if (property != null && property.CanWrite) {
					value = valueGetter(obj, new PropertyMemberInfo(property));
					property.FastSetValue(obj, value);
				} else {
					var field = type.GetField(propertyName, flags);
					if (field != null) {
						value = valueGetter(obj, new FieldMemberInfo(field));
						field.FastSetValue(obj, value);
					}
				}
			} catch (Exception e) {
				throw new ExceptionBase(-1, string.Format("SetPropertyValue Error: {0}\r\nPropertyName={1}, Value={2}, ValueType={3}", e.Message, propertyName, value, value == null ? null : value.GetType().Name), e);
			}
		}

		private class PropertyAccessorCollection { }
		/// <summary>
		/// 获取属性操作者字典（提高性能）
		/// </summary>
		/// <param name="type">对象类型</param>
		/// <returns>操作者字典</returns>
		public static IDictionary<string, IPropertyAccessor> GetPropertyAccessors(Type type) {
			if (type == null) {
				return null;
			}
			return type.GetObjectContext(typeof(PropertyAccessorCollection), () => {
				const BindingFlags flags = DefaultFieldBindingFlags | BindingFlags.Static;
				var properties = type.GetProperties(flags);
				var accessors = new List<IPropertyAccessor>();
				foreach (var property in properties) {
					accessors.Add(new PropertyAccessor(property));
				}
				return accessors.ToDictionary(k => k.Property.Name, v => v);
			});
		}

		#region ReflectionHelperInternal

		//just for dynamic load
		private class ReflectionHelperInternal : ReflectionHelper
		{
		}

		#endregion
	}
}

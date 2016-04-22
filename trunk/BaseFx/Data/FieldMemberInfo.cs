/* Copyright(c) 2005-2011 R2@DevFx.NET, License(LGPL) */

using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using HTB.DevFx.Data.Attributes;

namespace HTB.DevFx.Data
{
	/// <summary>
	/// 实现<see cref="IFieldMemberInfo"/>接口
	/// </summary>
	public class FieldMemberInfo : IFieldMemberInfo
	{
		#region constructor
		
		/// <summary>
		/// 构造方法
		/// </summary>
		/// <param name="columnAttribute">特性</param>
		/// <param name="property">属性信息</param>
		public FieldMemberInfo(ColumnAttribute columnAttribute, PropertyInfo property) {
			this.Column = columnAttribute;
			this.property = property;
			this.MemberInfo = property;
			this.Name = columnAttribute.ColumnName ?? property.Name;
			this.CanRead = !columnAttribute.WriteOnly && property.CanRead;
			this.CanWrite = !columnAttribute.ReadOnly && property.CanWrite;
		}

		/// <summary>
		/// 构造方法
		/// </summary>
		/// <param name="columnAttribute">特性</param>
		/// <param name="field">字段信息</param>
		public FieldMemberInfo(ColumnAttribute columnAttribute, FieldInfo field) {
			this.Column = columnAttribute;
			this.field = field;
			this.MemberInfo = field;
			this.Name = columnAttribute.ColumnName ?? field.Name;
			this.CanRead = !columnAttribute.WriteOnly;
			this.CanWrite = !columnAttribute.ReadOnly;
		}

		#endregion

		#region private members

		private PropertyInfo property;
		private FieldInfo field;

		#endregion

		#region IFieldMemberInfo Members

		/// <summary>
		/// 字段是否可读
		/// </summary>
		public bool CanRead { get; private set; }

		/// <summary>
		/// 字段是否可写
		/// </summary>
		public bool CanWrite { get; private set; }

		/// <summary>
		/// 字段名
		/// </summary>
		public string Name { get; private set; }

		/// <summary>
		/// 字段加载的成员信息<see cref="IFieldMemberInfo.MemberInfo"/>
		/// </summary>
		public MemberInfo MemberInfo { get; private set; }

		/// <summary>
		/// 字段本身的加载信息<see cref="ColumnAttribute"/>
		/// </summary>
		public ColumnAttribute Column { get; private set; }

		/// <summary>
		/// 获取字段的值
		/// </summary>
		/// <param name="obj">包含此字段的实例</param>
		/// <returns>字段的值</returns>
		public object GetValue(object obj) {
			return this.property != null ? this.property.GetValue(obj, null) : this.field.GetValue(obj);
		}

		/// <summary>
		/// 设置字段的值
		/// </summary>
		/// <param name="obj">包含此字段的实例</param>
		/// <param name="value">字段的值</param>
		public void SetValue(object obj, object value) {
			Type type;
			type = this.property != null ? this.property.PropertyType : this.field.FieldType;
			if(Convert.IsDBNull(value)) {
				if(this.Column.DefaultValue != null) {
					value = this.Column.DefaultValue;
				} else if(!type.IsValueType) {
					value = null;
				} else {
					return;
				}
			}
			if(this.property != null) {
				this.property.SetValue(obj, value, null);
			} else {
				this.field.SetValue(obj, value);
			}
		}

		#endregion

		#region static members

		private static readonly Dictionary<Type, IFieldMemberInfo[]> cache = new Dictionary<Type,IFieldMemberInfo[]>();
		private static readonly object lockObject = new object();

		/// <summary>
		/// 字段绑定预置值
		/// </summary>
		public const BindingFlags FieldBindingFlags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;

		/// <summary>
		/// 获取绑定的字段信息列表
		/// </summary>
		/// <param name="type">被获取的类型</param>
		/// <returns>IFieldMemberInfo[]</returns>
		public static IFieldMemberInfo[] GetFieldMembers(Type type) {
			return GetFieldMembers(type, FieldBindingFlags);
		}

		/// <summary>
		/// 获取绑定的字段信息列表
		/// </summary>
		/// <param name="type">被获取的类型</param>
		/// <param name="columnBindingFlags">绑定标识</param>
		/// <returns>IFieldMemberInfo[]</returns>
		public static IFieldMemberInfo[] GetFieldMembers(Type type, BindingFlags columnBindingFlags) {
			return GetFieldMembers(type, columnBindingFlags, false);
		}

		/// <summary>
		/// 获取绑定的字段信息列表
		/// </summary>
		/// <param name="type">被获取的类型</param>
		/// <param name="columnBindingFlags">绑定标识</param>
		/// <param name="inherit">是否包含继承类</param>
		/// <returns>IFieldMemberInfo[]</returns>
		public static IFieldMemberInfo[] GetFieldMembers(Type type, BindingFlags columnBindingFlags, bool inherit) {
			if(cache.ContainsKey(type)) {
				return cache[type];
			}
			lock (lockObject) {
				if (cache.ContainsKey(type)) {
					return cache[type];
				}
				var fieldMembers = new ArrayList();
				var props = type.GetProperties(columnBindingFlags);
				for (var i = 0; i < props.Length; i++) {
					var columnAttributes = (ColumnAttribute[])props[i].GetCustomAttributes(typeof(ColumnAttribute), inherit);
					if (columnAttributes != null && columnAttributes.Length > 0) {
						fieldMembers.Add(new FieldMemberInfo(columnAttributes[0], props[i]));
					}
				}
				var fields = type.GetFields(columnBindingFlags);
				for (var i = 0; i < fields.Length; i++) {
					var columnAttributes = (ColumnAttribute[])fields[i].GetCustomAttributes(typeof(ColumnAttribute), inherit);
					if (columnAttributes != null && columnAttributes.Length > 0) {
						fieldMembers.Add(new FieldMemberInfo(columnAttributes[0], fields[i]));
					}
				}
				var members = (IFieldMemberInfo[])fieldMembers.ToArray(typeof(IFieldMemberInfo));
				if (!cache.ContainsKey(type)) {
					cache.Add(type, members);
				}
				return members;
			}
		}

		/// <summary>
		/// 获取绑定的字段信息
		/// </summary>
		/// <param name="type">被获取的类型</param>
		/// <param name="fieldName">字段名</param>
		/// <param name="columnBindingFlags">绑定标识</param>
		/// <param name="inherit">是否包含继承类</param>
		/// <returns>IFieldMemberInfo</returns>
		public static IFieldMemberInfo GetFieldMember(Type type, string fieldName, BindingFlags columnBindingFlags, bool inherit) {
			var prop = type.GetProperty(fieldName, columnBindingFlags);
			if(prop != null) {
				var columnAttributes = (ColumnAttribute[])prop.GetCustomAttributes(typeof(ColumnAttribute), inherit);
				if(columnAttributes != null && columnAttributes.Length > 0) {
					return new FieldMemberInfo(columnAttributes[0], prop);
				}
			}

			var field = type.GetField(fieldName, columnBindingFlags);
			if(field != null) {
				var columnAttributes = (ColumnAttribute[])field.GetCustomAttributes(typeof(ColumnAttribute), inherit);
				if(columnAttributes != null && columnAttributes.Length > 0) {
					return new FieldMemberInfo(columnAttributes[0], field);
				}
			}

			return null;
		}

		#endregion
	}
}

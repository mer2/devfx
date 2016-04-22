/* Copyright(c) 2005-2011 R2@DevFx.NET, License(LGPL) */

using System;
using System.Data;
using HTB.DevFx.Data.Attributes;
using HTB.DevFx.Utils;

namespace HTB.DevFx.Data.Utils
{
	/// <summary>
	/// 关于数据访问的一些有用的方法
	/// </summary>
	public static class DataHelper
	{
		private static object GetValueSafely(DataRow row, string fieldName, object value, object defaultValue) {
			if (Convert.IsDBNull(value)) {
				DataColumn column = row.Table.Columns[fieldName];
				if (!Convert.IsDBNull(column.DefaultValue)) {
					value = column.DefaultValue;
				} else {
					value = defaultValue;
				}
			}
			return value;
		}

		/// <summary>
		/// 安全的获取DataRow指定列的值
		/// </summary>
		/// <param name="row">DataRow</param>
		/// <param name="fieldName">列名</param>
		/// <param name="defaultValue">此列的默认值（如果获取失败，则返回此默认值）</param>
		/// <returns>列的值</returns>
		public static object GetValueSafely(DataRow row, string fieldName, object defaultValue) {
			return GetValueSafely(row, fieldName, row[fieldName], defaultValue);
		}

		/// <summary>
		/// 安全的获取DataRow指定列指定版本的值
		/// </summary>
		/// <param name="row">DataRow</param>
		/// <param name="fieldName">列名</param>
		/// <param name="rowVersion">列版本</param>
		/// <param name="defaultValue">此列的默认值（如果获取失败，则返回此默认值）</param>
		/// <returns>列的值</returns>
		public static object GetValueSafely(DataRow row, string fieldName, DataRowVersion rowVersion, object defaultValue) {
			return GetValueSafely(row, fieldName, row[fieldName, rowVersion], defaultValue);
		}

		/// <summary>
		/// 设置DataRow的各列值为缺省值
		/// </summary>
		/// <param name="dr">DataRow</param>
		/// <param name="values">缺省值列表，与DataRow的列具有相同的个数和顺序</param>
		public static void SetDefaultValueWhenDBNull(DataRow dr, object[] values) {
			if(dr.Table.Columns.Count != values.Length) {
				return;
			}
			for(int i = 0; i < dr.Table.Columns.Count; i++) {
				string fieldName = dr.Table.Columns[i].ColumnName;
				if(!dr.Table.Columns[i].AllowDBNull && Convert.IsDBNull(dr[fieldName])) {
					if(!Convert.IsDBNull(dr.Table.Columns[i].DefaultValue)) {
						dr[fieldName] = dr.Table.Columns[i].DefaultValue;
					} else {
						dr[fieldName] = values[i];
					}
				}
			}
		}

		/// <summary>
		/// 设置DataRow的各列值为缺省值
		/// </summary>
		/// <param name="dr">DataRow</param>
		/// <param name="values">缺省值列表，与DataRow的列具有相同的个数和顺序</param>
		public static void SetDataRowDefaultValues(DataRow dr, object[] values) {
			if(values == null || values.Length <= 0 || dr == null) {
				return;
			}
			int num = values.Length;
			if(dr.Table.Columns.Count < num) {
				num = dr.Table.Columns.Count;
			}
			for (int i = 0; i < num; i++) {
				if (!dr.Table.Columns[i].ReadOnly) {
					dr[i] = values[i];
				}
			}
		}

		/// <summary>
		/// 设置DataRow的各列值为缺省值（从DataTable的Schema中读取缺省值）
		/// </summary>
		/// <param name="dr">DataRow</param>
		public static void SetDefaultValueWhenDBNull(DataRow dr) {
			for(int i = 0; i < dr.Table.Columns.Count; i++) {
				string fieldName = dr.Table.Columns[i].ColumnName;
				if(!dr.Table.Columns[i].AllowDBNull && Convert.IsDBNull(dr[fieldName])) {
					if(!Convert.IsDBNull(dr.Table.Columns[i].DefaultValue)) {
						dr[fieldName] = dr.Table.Columns[i].DefaultValue;
					} else {
						dr[fieldName] = TypeHelper.CreateObject(dr.Table.Columns[i].DataType, null, true);
					}
				}
			}
		}

		/// <summary>
		/// 设置DataTable列的缺省值
		/// </summary>
		/// <param name="dt">DataTable</param>
		/// <param name="values">缺省值列表，与DataTable的列具有相同顺序（允许个数不同）</param>
		public static void SetColumnDefaultValue(DataTable dt, object[] values) {
			Checker.CheckArgumentNull("DataTable", dt, true);
			Checker.CheckEmptyArray("values", values, true);
			int num = dt.Columns.Count;
			if(values.Length < num) {
				num = values.Length;
			}
			for(int i = 0; i < num; i++) {
				if(!dt.Columns[i].AllowDBNull && !dt.Columns[i].AutoIncrement && Convert.IsDBNull(dt.Columns[i].DefaultValue)) {
					dt.Columns[i].DefaultValue = values[i];
				}
			}
		}

		/// <summary>
		/// 设置DataTable列是否允许DBNull
		/// </summary>
		/// <param name="dt">DataTable</param>
		/// <param name="values">列列是否允许DBNull，与DataTable的列具有相同顺序（允许个数不同）</param>
		public static void SetColumnAllowDBNull(DataTable dt, bool[] values) {
			Checker.CheckArgumentNull("DataTable", dt, true);
			Checker.CheckEmptyArray("values", values, true);
			int num = dt.Columns.Count;
			if(values.Length < num) {
				num = values.Length;
			}
			for(int i = 0; i < num; i++) {
				dt.Columns[i].AllowDBNull = values[i];
			}
		}
		
		/// <summary>
		/// 获取具有 <see cref="ColumnAttribute"/> 属性的缺省值
		/// </summary>
		/// <param name="type">需要反射的类型</param>
		/// <returns>缺省值列表</returns>
		public static object[] GetDefaultColumnAttibuteValues(Type type) {
			IFieldMemberInfo[] members = FieldMemberInfo.GetFieldMembers(type);
			object[] defaultValues = new object[members.Length];
			for(int i = 0; i < members.Length; i++) {
				IFieldMemberInfo member = members[i];
				Type memberType = member.MemberInfo.DeclaringType;
				object defaultValue = member.Column.DefaultValue;
				if (defaultValue == null) {
					defaultValues[i] = memberType.TypeInitializer.Invoke(null);
				} else {
					if(memberType == typeof(DateTime)) {
						switch(defaultValue.ToString()) {
							case "DateTime.Now":
								defaultValue = DateTime.Now;
								break;
							case "DateTime.MinValue":
								defaultValue = DateTime.MinValue;
								break;
							case "DateTime.MaxValue":
								defaultValue = DateTime.MaxValue;
								break;
						}
					}
					defaultValues[i] = defaultValue;
				}
			}
			return defaultValues;
		}
	}
}

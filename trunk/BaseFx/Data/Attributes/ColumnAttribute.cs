/* Copyright(c) 2005-2011 R2@DevFx.NET, License(LGPL) */

using System;

namespace HTB.DevFx.Data.Attributes
{
	/// <summary>
	/// 一个简单实现O/R关系的描述属性
	/// </summary>
	/// <remarks>
	/// 配合 <see cref="HTB.DevFx.Data.Utils.DataTransfer"/> 来实现对象和数据的转换
	/// </remarks>
	[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple=false)]
	public class ColumnAttribute : Attribute
	{
		/// <summary>
		/// 构造函数
		/// </summary>
		public ColumnAttribute() {
			this.IsNullable = true;
		}

		/// <summary>
		/// 构造函数
		/// </summary>
		/// <param name="columnName">列名</param>
		public ColumnAttribute(string columnName) : this() {
			this.ColumnName = columnName;
		}

		/// <summary>
		/// 设置/获取列名
		/// </summary>
		public string ColumnName { get; set; }

		/// <summary>
		/// 设置/获取列隶属的组分类
		/// </summary>
		public string ColumnGroup { get; set; }

		/// <summary>
		/// 设置/获取列的类型
		/// </summary>
		public Type ColumnType { get; set; }

		/// <summary>
		/// 设置/获取列类型的长度
		/// </summary>
		public int ColumnSize { get; set; }

		/// <summary>
		/// 设置/获取列的缺省值
		/// </summary>
		public object DefaultValue { get; set; }

		/// <summary>
		/// 设置/获取指示此列是否只读
		/// </summary>
		public bool ReadOnly { get; set; }

		/// <summary>
		/// 设置/获取指示此列是否只写
		/// </summary>
		public bool WriteOnly { get; set; }

		/// <summary>
		/// 是否为主键
		/// </summary>
		public bool IsPrimaryKey { get; set; }

		/// <summary>
		/// 是否可<c>null</c>
		/// </summary>
		public bool IsNullable { get; set; }
	}
}

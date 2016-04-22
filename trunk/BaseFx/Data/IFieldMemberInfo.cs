/* Copyright(c) 2005-2011 R2@DevFx.NET, License(LGPL) */

using System.Reflection;
using HTB.DevFx.Data.Attributes;

namespace HTB.DevFx.Data
{
	/// <summary>
	/// 对<see cref="ColumnAttribute"/>的包装
	/// </summary>
	public interface IFieldMemberInfo
	{
		/// <summary>
		/// 字段是否可读
		/// </summary>
		bool CanRead { get; }

		/// <summary>
		/// 字段是否可写
		/// </summary>
		bool CanWrite { get; }

		/// <summary>
		/// 字段名
		/// </summary>
		string Name { get; }

		/// <summary>
		/// 字段加载的成员信息<see cref="MemberInfo"/>
		/// </summary>
		MemberInfo MemberInfo { get; }

		/// <summary>
		/// 字段本身的加载信息<see cref="ColumnAttribute"/>
		/// </summary>
		ColumnAttribute Column { get; }

		/// <summary>
		/// 获取字段的值
		/// </summary>
		/// <param name="obj">包含此字段的实例</param>
		/// <returns>字段的值</returns>
		object GetValue(object obj);

		/// <summary>
		/// 设置字段的值
		/// </summary>
		/// <param name="obj">包含此字段的实例</param>
		/// <param name="value">字段的值</param>
		void SetValue(object obj, object value);
	}
}

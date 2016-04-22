/* Copyright(c) 2005-2011 R2@DevFx.NET, License(LGPL) */

using System;
using System.Reflection;

namespace HTB.DevFx.Reflection
{
	/// <summary>
	/// 字段或属性信息
	/// </summary>
	public interface IFieldOrPropertyInfo
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
		/// 字段类型
		/// </summary>
		Type MemberType { get; }

		/// <summary>
		/// 字段加载的成员信息<see cref="MemberInfo"/>
		/// </summary>
		MemberInfo MemberInfo { get; }
	}
}

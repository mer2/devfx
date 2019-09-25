/* Copyright(c) 2005-2011 R2@DevFx.NET, License(LGPL) */

using System;
using System.Reflection;

namespace DevFx.Reflection
{
	internal class PropertyMemberInfo : IFieldOrPropertyInfo
	{
		public PropertyMemberInfo(PropertyInfo propertyInfo) {
			this.propertyInfo = propertyInfo;
		}

		private readonly PropertyInfo propertyInfo;

		#region Implementation of IMemberInfo

		/// <inheritdoc />
		/// <summary>
		/// 字段是否可读
		/// </summary>
		public bool CanRead => this.propertyInfo.CanRead;

		/// <inheritdoc />
		/// <summary>
		/// 字段是否可写
		/// </summary>
		public bool CanWrite => this.propertyInfo.CanWrite;

		/// <inheritdoc />
		/// <summary>
		/// 成员名称
		/// </summary>
		public string Name => this.propertyInfo.Name;

		/// <inheritdoc />
		/// <summary>
		/// 成员类型
		/// </summary>
		public Type MemberType => this.propertyInfo.PropertyType;

		/// <inheritdoc />
		/// <summary>
		/// 成员信息
		/// </summary>
		public MemberInfo MemberInfo => this.propertyInfo;

		#endregion
	}
}

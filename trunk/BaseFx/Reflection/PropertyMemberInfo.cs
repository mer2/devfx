/* Copyright(c) 2005-2011 R2@DevFx.NET, License(LGPL) */

using System;
using System.Reflection;

namespace HTB.DevFx.Reflection
{
	internal class PropertyMemberInfo : IFieldOrPropertyInfo
	{
		public PropertyMemberInfo(PropertyInfo propertyInfo) {
			this.propertyInfo = propertyInfo;
		}

		private readonly PropertyInfo propertyInfo;

		#region Implementation of IMemberInfo

		/// <summary>
		/// 字段是否可读
		/// </summary>
		public bool CanRead {
			get { return this.propertyInfo.CanRead; }
		}

		/// <summary>
		/// 字段是否可写
		/// </summary>
		public bool CanWrite {
			get { return this.propertyInfo.CanWrite; }
		}

		/// <summary>
		/// 成员名称
		/// </summary>
		public string Name {
			get { return this.propertyInfo.Name; }
		}

		/// <summary>
		/// 成员类型
		/// </summary>
		public Type MemberType {
			get { return this.propertyInfo.PropertyType; }
		}

		/// <summary>
		/// 成员信息
		/// </summary>
		public MemberInfo MemberInfo {
			get { return this.propertyInfo; }
		}

		#endregion
	}
}

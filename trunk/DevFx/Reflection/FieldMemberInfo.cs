/* Copyright(c) 2005-2011 R2@DevFx.NET, License(LGPL) */

using System;
using System.Reflection;

namespace HTB.DevFx.Reflection
{
	internal class FieldMemberInfo : IFieldOrPropertyInfo
	{
		public FieldMemberInfo(FieldInfo fieldInfo) {
			this.fieldInfo = fieldInfo;
		}

		private readonly FieldInfo fieldInfo;

		#region Implementation of IMemberInfo

		/// <summary>
		/// 字段是否可读
		/// </summary>
		public bool CanRead {
			get { return true; }
		}

		/// <summary>
		/// 字段是否可写
		/// </summary>
		public bool CanWrite {
			get { return true; }
		}

		/// <summary>
		/// 成员名称
		/// </summary>
		public string Name {
			get { return this.fieldInfo.Name; }
		}

		/// <summary>
		/// 成员类型
		/// </summary>
		public Type MemberType {
			get { return this.fieldInfo.FieldType; }
		}

		/// <summary>
		/// 成员信息
		/// </summary>
		public MemberInfo MemberInfo {
			get { return this.fieldInfo; }
		}

		#endregion
	}
}

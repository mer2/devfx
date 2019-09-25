/* Copyright(c) 2005-2011 R2@DevFx.NET, License(LGPL) */

using System;
using System.Reflection;
using System.Linq.Expressions;

namespace DevFx.Reflection
{
	internal class FieldAccessor : IFieldAccessor
	{
		public FieldAccessor(FieldInfo fieldInfo) {
			this.FieldInfo = fieldInfo;
			this.fieldOrPropertyInfo = new FieldMemberInfo(fieldInfo);
			this.InitializeGet(fieldInfo);
		}

		public object GetValue(object instance) {
			return this.getter(instance);
		}

		public void SetValue(object o, object value) {
			this.FieldInfo.SetValue(o, value);
		}

		public FieldInfo FieldInfo { get; }

		private readonly IFieldOrPropertyInfo fieldOrPropertyInfo;
		private Func<object, object> getter;

		private void InitializeGet(FieldInfo fieldInfo) {
			// target: (object)(((TInstance)instance).Field)

			// preparing parameter, object type
			var instance = Expression.Parameter(typeof(object), "instance");

			// non-instance for static method, or ((TInstance)instance)
			var instanceCast = fieldInfo.IsStatic ? null : Expression.Convert(instance, fieldInfo.ReflectedType);

			// ((TInstance)instance).Property
			var fieldAccess = Expression.Field(instanceCast, fieldInfo);

			// (object)(((TInstance)instance).Property)
			var castFieldValue = Expression.Convert(fieldAccess, typeof(object));

			// Lambda expression
			var lambda = Expression.Lambda<Func<object, object>>(castFieldValue, instance);

			this.getter = lambda.Compile();
		}

		#region IFieldAccessor Members

		IFieldOrPropertyInfo IPropertyAccessor.Property {
			get { return this.fieldOrPropertyInfo; }
		}

		object IPropertyAccessor.GetValue(object instance) {
			return this.GetValue(instance);
		}

		void IPropertyAccessor.SetValue(object instance, object value) {
			this.SetValue(instance, value);
		}

		#endregion
	}
}
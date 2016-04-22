/* Copyright(c) 2005-2011 R2@DevFx.NET, License(LGPL) */

using System;
using System.Linq.Expressions;
using System.Reflection;

namespace HTB.DevFx.Reflection
{
	internal class PropertyAccessor : IPropertyAccessor
	{
		public PropertyAccessor(PropertyInfo propertyInfo) {
			this.PropertyInfo = propertyInfo;
			this.fieldOrPropertyInfo = new PropertyMemberInfo(propertyInfo);
			this.InitializeGet(propertyInfo);
			this.InitializeSet(propertyInfo);
		}

		public object GetValue(object o) {
			if (this.getter == null) {
				throw new NotSupportedException("Get method is not defined for this property.");
			}
			return this.getter(o);
		}

		public void SetValue(object o, object value) {
			if (this.setter == null) {
				throw new NotSupportedException("Set method is not defined for this property.");
			}
			this.setter.Invoke(o, new[] { value });
		}

		public PropertyInfo PropertyInfo { get; private set; }

		private IFieldOrPropertyInfo fieldOrPropertyInfo;
		private Func<object, object> getter;
		private MethodInvoker setter;

		private void InitializeGet(PropertyInfo propertyInfo) {
			if (!propertyInfo.CanRead) return;

			// Target: (object)(((TInstance)instance).Property)

			// preparing parameter, object type
			var instance = Expression.Parameter(typeof(object), "instance");

			// non-instance for static method, or ((TInstance)instance)
			var instanceCast = propertyInfo.GetGetMethod(true).IsStatic ? null :
																				Expression.Convert(instance, propertyInfo.ReflectedType);

			// ((TInstance)instance).Property
			var propertyAccess = Expression.Property(instanceCast, propertyInfo);

			// (object)(((TInstance)instance).Property)
			var castPropertyValue = Expression.Convert(propertyAccess, typeof(object));

			// Lambda expression
			var lambda = Expression.Lambda<Func<object, object>>(castPropertyValue, instance);

			this.getter = lambda.Compile();
		}

		private void InitializeSet(PropertyInfo propertyInfo) {
			if (!propertyInfo.CanWrite) return;
			this.setter = new MethodInvoker(propertyInfo.GetSetMethod(true));
		}

		#region IPropertyAccessor Members

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
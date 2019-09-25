/* Copyright(c) 2005-2011 R2@DevFx.NET, License(LGPL) */

using System.Reflection;

namespace DevFx.Reflection
{
	internal class PropertyAccessorFactory : IFastReflectionFactory<PropertyInfo, IPropertyAccessor>
	{
		public IPropertyAccessor Create(PropertyInfo key) {
			return new PropertyAccessor(key);
		}

		#region IFastReflectionFactory<PropertyInfo,IPropertyAccessor> Members

		IPropertyAccessor IFastReflectionFactory<PropertyInfo, IPropertyAccessor>.Create(PropertyInfo key) {
			return this.Create(key);
		}

		#endregion
	}
}
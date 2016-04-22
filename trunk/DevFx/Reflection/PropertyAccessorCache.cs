/* Copyright(c) 2005-2011 R2@DevFx.NET, License(LGPL) */

using System.Reflection;

namespace HTB.DevFx.Reflection
{
	internal class PropertyAccessorCache : FastReflectionCache<PropertyInfo, IPropertyAccessor>
	{
		protected override IPropertyAccessor Create(PropertyInfo key) {
			return new PropertyAccessor(key);
		}
	}
}
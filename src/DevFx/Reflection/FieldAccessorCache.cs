/* Copyright(c) 2005-2011 R2@DevFx.NET, License(LGPL) */

using System.Reflection;

namespace DevFx.Reflection
{
	internal class FieldAccessorCache : FastReflectionCache<FieldInfo, IFieldAccessor>
	{
		protected override IFieldAccessor Create(FieldInfo key) {
			return FastReflectionFactories.FieldAccessorFactory.Create(key);
		}
	}
}
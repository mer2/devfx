/* Copyright(c) 2005-2011 R2@DevFx.NET, License(LGPL) */

using System.Reflection;

namespace DevFx.Reflection
{
	internal class FieldAccessorFactory : IFastReflectionFactory<FieldInfo, IFieldAccessor>
	{
		public IFieldAccessor Create(FieldInfo key) {
			return new FieldAccessor(key);
		}

		#region IFastReflectionFactory<FieldInfo,IFieldAccessor> Members

		IFieldAccessor IFastReflectionFactory<FieldInfo, IFieldAccessor>.Create(FieldInfo key) {
			return this.Create(key);
		}

		#endregion
	}
}
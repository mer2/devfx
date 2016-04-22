/* Copyright(c) 2005-2011 R2@DevFx.NET, License(LGPL) */

using System.Reflection;

namespace HTB.DevFx.Reflection
{
	internal class ConstructorInvokerFactory : IFastReflectionFactory<ConstructorInfo, IConstructorInvoker>
	{
		public IConstructorInvoker Create(ConstructorInfo key) {
			return new ConstructorInvoker(key);
		}

		#region IFastReflectionFactory<ConstructorInfo,IConstructorInvoker> Members

		IConstructorInvoker IFastReflectionFactory<ConstructorInfo, IConstructorInvoker>.Create(ConstructorInfo key) {
			return this.Create(key);
		}

		#endregion
	}
}
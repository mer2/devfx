/* Copyright(c) 2005-2011 R2@DevFx.NET, License(LGPL) */

using System.Reflection;

namespace DevFx.Reflection
{
	internal class MethodInvokerFactory : IFastReflectionFactory<MethodInfo, IMethodInvoker>
	{
		public IMethodInvoker Create(MethodInfo key) {
			return new MethodInvoker(key);
		}

		#region IFastReflectionFactory<MethodInfo,IMethodInvoker> Members

		IMethodInvoker IFastReflectionFactory<MethodInfo, IMethodInvoker>.Create(MethodInfo key) {
			return this.Create(key);
		}

		#endregion
	}
}
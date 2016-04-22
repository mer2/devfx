/* Copyright(c) 2005-2011 R2@DevFx.NET, License(LGPL) */

using System.Reflection;

namespace HTB.DevFx.Reflection
{
	internal class MethodInvokerCache : FastReflectionCache<MethodInfo, IMethodInvoker>
	{
		protected override IMethodInvoker Create(MethodInfo key) {
			return FastReflectionFactories.MethodInvokerFactory.Create(key);
		}
	}
}
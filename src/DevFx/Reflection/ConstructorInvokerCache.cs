/* Copyright(c) 2005-2011 R2@DevFx.NET, License(LGPL) */

using System.Reflection;

namespace DevFx.Reflection
{
	internal class ConstructorInvokerCache : FastReflectionCache<ConstructorInfo, IConstructorInvoker>
	{
		protected override IConstructorInvoker Create(ConstructorInfo key) {
			return FastReflectionFactories.ConstructorInvokerFactory.Create(key);
		}
	}
}
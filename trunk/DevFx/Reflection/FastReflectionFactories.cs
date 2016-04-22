/* Copyright(c) 2005-2011 R2@DevFx.NET, License(LGPL) */

using System.Reflection;

namespace HTB.DevFx.Reflection
{
	internal static class FastReflectionFactories
	{
		static FastReflectionFactories() {
			MethodInvokerFactory = new MethodInvokerFactory();
			PropertyAccessorFactory = new PropertyAccessorFactory();
			FieldAccessorFactory = new FieldAccessorFactory();
			ConstructorInvokerFactory = new ConstructorInvokerFactory();
		}

		public static IFastReflectionFactory<MethodInfo, IMethodInvoker> MethodInvokerFactory { get; set; }
		public static IFastReflectionFactory<PropertyInfo, IPropertyAccessor> PropertyAccessorFactory { get; set; }
		public static IFastReflectionFactory<FieldInfo, IFieldAccessor> FieldAccessorFactory { get; set; }
		public static IFastReflectionFactory<ConstructorInfo, IConstructorInvoker> ConstructorInvokerFactory { get; set; }
	}
}
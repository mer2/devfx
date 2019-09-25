/* Copyright(c) 2005-2011 R2@DevFx.NET, License(LGPL) */

using System.Reflection;

namespace DevFx.Reflection
{
	internal static class FastReflectionExtensions
	{
		public static object FastInvoke(this MethodInfo methodInfo, object instance, params object[] parameters) {
			return FastReflectionCaches.MethodInvokerCache.Get(methodInfo).Invoke(instance, parameters);
		}

		public static object FastGetValue(this PropertyInfo propertyInfo, object instance) {
			return FastReflectionCaches.PropertyAccessorCache.Get(propertyInfo).GetValue(instance);
		}

		public static void FastSetValue(this PropertyInfo propertyInfo, object instance, object value) {
			FastReflectionCaches.PropertyAccessorCache.Get(propertyInfo).SetValue(instance, value);
		}

		public static object FastGetValue(this FieldInfo fieldInfo, object instance) {
			return FastReflectionCaches.FieldAccessorCache.Get(fieldInfo).GetValue(instance);
		}

		public static void FastSetValue(this FieldInfo fieldInfo, object instance, object value) {
			FastReflectionCaches.FieldAccessorCache.Get(fieldInfo).SetValue(instance, value);
		}

		public static object FastInvoke(this ConstructorInfo constructorInfo, params object[] parameters) {
			return FastReflectionCaches.ConstructorInvokerCache.Get(constructorInfo).Invoke(parameters);
		}
	}
}
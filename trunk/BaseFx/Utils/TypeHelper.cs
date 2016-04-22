/* Copyright(c) 2005-2011 R2@DevFx.NET, License(LGPL) */

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using HTB.DevFx.Exceptions;

namespace HTB.DevFx.Utils
{
	/// <summary>
	/// 关于类型、实例的一些实用方法
	/// </summary>
	public abstract partial class TypeHelper
	{
		internal TypeHelper() { }
		static TypeHelper() {
			ConstructorInvoker = ConstructorInvoke;
			MethodInvoker = MethodInvoke;
			//尝试调用后续类
			var reflectionHelper = CreateObject("HTB.DevFx.Utils.ReflectionHelper+ReflectionHelperInternal, HTB.DevFx", null, false);
			if(reflectionHelper != null) {
				reflectionHelper.ToString();
			}
		}
		static object ConstructorInvoke(ConstructorInfo ctor, object[] parameterValues) {
			return ctor.Invoke(parameterValues);
		}
		static object MethodInvoke(MethodInfo method, object instance, object[] parameterValues) {
			return method.Invoke(instance, parameterValues);
		}

		internal const BindingFlags DefaultFieldBindingFlags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;
		internal static object ConstructorInvokerLockObject = new object();
		internal static object MethodInvokerLockObject = new object();
		internal static Func<ConstructorInfo, object[], object> ConstructorInvoker;
		internal static Func<MethodInfo, object, object[], object> MethodInvoker;

		/// <summary>
		/// 从类型名称中创建类型
		/// </summary>
		/// <param name="typeName">类型名</param>
		/// <param name="throwOnError">失败时是否抛出异常</param>
		/// <returns>Type</returns>
		public static Type CreateType(string typeName, bool throwOnError) {
			return Type.GetType(typeName, throwOnError, false);
		}

		/// <summary>
		/// 从类型名称中创建类型
		/// </summary>
		/// <param name="typeName">类型名</param>
		/// <param name="expectedType">期望的类型</param>
		/// <param name="throwOnError">失败时是否抛出异常</param>
		/// <returns>Type</returns>
		public static Type CreateType(string typeName, Type expectedType, bool throwOnError) {
			var type = CreateType(typeName, throwOnError);
			if (expectedType != null && !expectedType.IsAssignableFrom(type)) {
				if (throwOnError) {
					throw new Exception(string.Format("将要创建的类型：{0}，不是期望的类型：{1}", type.FullName, expectedType.FullName));
				}
				return null;
			}
			return type;
		}

		internal static Assembly AssemblyLoad(AssemblyName assemblyName, bool throwOnError) {
			Assembly assembly = null;
			try {
				assembly = Assembly.Load(assemblyName);
			} catch(Exception e) {
				if(e is ArgumentNullException || throwOnError) {
					throw;
				}
			}
			return assembly;
		}

		internal static Assembly AssemblyLoad(string assemblyName, bool throwOnError) {
			Assembly assembly = null;
			try {
				assembly = Assembly.Load(assemblyName);
			} catch (Exception e) {
				if (e is ArgumentNullException || throwOnError) {
					throw;
				}
			}
			return assembly;
		}

		/// <summary>
		/// 根据元属性获取应用此元属性的程序集列表
		/// </summary>
		/// <param name="attributeType">元属性类型</param>
		/// <returns>程序集列表</returns>
		public static Assembly[] GetAssembliesByCustomAttribute(Type attributeType) {
			var assemblies = LoadAssembliesFromBin();
			var list = new List<Assembly>();
			foreach (var assembly in assemblies) {
				if (assembly.IsDefined(attributeType, false)) {
					list.Add(assembly);
				}
			}
			return list.ToArray();
		}

		/// <summary>
		/// 从运行时的堆栈中获取元属性
		/// </summary>
		/// <param name="includeAll">是否包含堆栈上所有的元属性</param>
		/// <typeparam name="T">元属性类型</typeparam>
		/// <returns>找到的元属性的数组</returns>
		public static T[] GetAttributeFromRuntimeStack<T>(bool includeAll) where T : Attribute {
			var list = new List<T>();
			var t = new StackTrace(false);
			for (var i = 0; i < t.FrameCount; i++) {
				var f = t.GetFrame(i);
				var m = f.GetMethod();
				var a = Attribute.GetCustomAttributes(m, typeof(T)) as T[];
				if (a != null && a.Length > 0) {
					list.AddRange(a);
					if (!includeAll) {
						break;
					}
				}
			}
			return list.ToArray();
		}

		/// <summary>
		/// 从类型中创建此类型的实例
		/// </summary>
		/// <param name="type">类型</param>
		/// <param name="expectedType">期望的类型</param>
		/// <param name="throwOnError">失败时是否抛出异常</param>
		/// <param name="parameterTypes">创建实例所需参数的类型列表</param>
		/// <param name="parameterValues">创建实例所需的参数值列表</param>
		/// <returns>类型实例</returns>
		public static object CreateObject(Type type, Type expectedType, bool throwOnError, Type[] parameterTypes, object[] parameterValues) {
			if (parameterTypes != null && parameterValues != null && parameterTypes.Length != parameterValues.Length) {
				if (throwOnError) {
					throw new Exception("构造函数参数类型数量和参数数量不一致");
				}
				return null;
			}
			return CreateObjectInternal(type, expectedType, throwOnError, parameterTypes, parameterValues);
		}

		internal static object CreateObjectInternal(Type type, Type expectedType, bool throwOnError, Type[] parameterTypes, object[] parameterValues) {
			if(type == null) {
				if(throwOnError) {
					throw new ArgumentNullException("type");
				}
				return null;
			}
			if(expectedType != null && !expectedType.IsAssignableFrom(type)) {
				if (throwOnError) {
					throw new Exception(string.Format("将要创建的类型：{0}，不是期望的类型：{1}", type.FullName, expectedType.FullName));
				}
				return null;
			}
			object createdObject = null;
			if (parameterTypes == null) {
				parameterTypes = new Type[] { };
			}
			var constructor = type.GetConstructor(parameterTypes);
			if (constructor == null) {
				try {
					createdObject = Activator.CreateInstance(type, BindingFlags.CreateInstance | (BindingFlags.NonPublic | (BindingFlags.Public | BindingFlags.Instance)), null, parameterValues, null);
				} catch (Exception e) {
					if (throwOnError) {
						throw new Exception("即将创建的类型不支持指定的构造函数：" + e.Message, e);
					}
				}
			} else {
				try {
					createdObject = ConstructorInvoker(constructor, parameterValues);
				} catch (Exception e) {
					throw new Exception("对象创建失败：" + e.Message, e);
				}
			}
			return createdObject;
		}

		/// <summary>
		/// 从类型中创建此类型的实例（本方法不支持参数可为Null的构造函数）
		/// </summary>
		/// <param name="type">类型</param>
		/// <param name="expectedType">期望的类型</param>
		/// <param name="throwOnError">失败时是否抛出异常</param>
		/// <param name="parameters">创建实例所需的参数值列表</param>
		/// <returns>类型实例</returns>
		public static object CreateObject(Type type, Type expectedType, bool throwOnError, params object[] parameters) {
			Type[] paramTypes = null;
			object[] paramValues = null;
			if (parameters != null) {
				var paramNum = parameters.Length;
				paramTypes = new Type[paramNum];
				paramValues = new object[paramNum];
				for (var i = 0; i < paramNum; i++) {
					if (parameters[i] == null) {
						if (throwOnError) {
							throw new Exception("不支持参数可为Null的构造函数，请使用本方法的另外重载版本");
						}
						return null;
					}
					paramTypes[i] = parameters[i].GetType();
					paramValues[i] = parameters[i];
				}
			}
			return CreateObjectInternal(type, expectedType, throwOnError, paramTypes, paramValues);
		}

		/// <summary>
		/// 从类型名中创建此类型的实例
		/// </summary>
		/// <param name="typeName">类型名</param>
		/// <param name="expectedType">期望的类型</param>
		/// <param name="throwOnError">失败时是否抛出异常</param>
		/// <param name="parameters">创建实例所需的参数值列表</param>
		/// <returns>类型实例</returns>
		public static object CreateObject(string typeName, Type expectedType, bool throwOnError, params object[] parameters) {
			var type = CreateType(typeName, throwOnError);
			return CreateObject(type, expectedType, throwOnError, parameters);
		}

		/// <summary>
		/// 从类型名中创建此类型的实例
		/// </summary>
		/// <param name="typeName">类型名</param>
		/// <param name="expectedType">期望的类型</param>
		/// <param name="throwOnError">失败时是否抛出异常</param>
		/// <param name="parameterTypes">创建实例所需参数的类型列表</param>
		/// <param name="parameterValues">创建实例所需的参数值列表</param>
		/// <returns>类型实例</returns>
		public static object CreateObject(string typeName, Type expectedType, bool throwOnError, Type[] parameterTypes, object[] parameterValues) {
			var type = CreateType(typeName, throwOnError);
			return CreateObject(type, expectedType, throwOnError, parameterTypes, parameterValues);
		}

		/// <summary>
		/// 使用反射调用方法
		/// </summary>
		/// <param name="instance">类型实例</param>
		/// <param name="methodName">方法名</param>
		/// <param name="parameters">参数列表</param>
		/// <returns>方法返回值</returns>
		public static object Invoke(object instance, string methodName, params object[] parameters) {
			object returnValue;
			TryInvoke(instance, methodName, out returnValue, true, parameters);
			return returnValue;
		}

		/// <summary>
		/// 尝试使用反射调用方法
		/// </summary>
		/// <param name="instance">类型实例</param>
		/// <param name="methodName">方法名</param>
		/// <param name="returnValue">方法返回值</param>
		/// <param name="throwOnError">失败时是否抛出异常</param>
		/// <param name="parameters">参数列表</param>
		/// <returns>是否调用成功</returns>
		public static bool TryInvoke(object instance, string methodName, out object returnValue, bool throwOnError, params object[] parameters) {
			returnValue = null;
			if (instance == null) {
				return false;
			}

			Type type;
			var flags = DefaultFieldBindingFlags;
			if (instance is Type) {
				type = (Type)instance;
				flags |= BindingFlags.Static;
			} else {
				type = instance.GetType();
			}
			MethodInfo method;
			try {
				method = type.GetMethod(methodName, flags);
			} catch (AmbiguousMatchException) {
				method = type.GetMethod(methodName, flags, null, GetTypes(parameters), null);
			}
			return TryInvoke(instance, method, out returnValue, throwOnError, parameters);
		}

		/// <summary>
		/// 尝试使用反射调用方法
		/// </summary>
		/// <param name="instance">类型实例</param>
		/// <param name="method">方法</param>
		/// <param name="returnValue">方法返回值</param>
		/// <param name="throwOnError">失败时是否抛出异常</param>
		/// <param name="parameters">参数列表</param>
		/// <returns>是否调用成功</returns>
		public static bool TryInvoke(object instance, MethodInfo method, out object returnValue, bool throwOnError, params object[] parameters) {
			returnValue = null;
			if(instance == null) {
				return false;
			}
			if(method != null) {
				try {
					returnValue = MethodInvoker(method, instance, parameters);
					return true;
				} catch {
					if(throwOnError) {
						throw;
					}
				}
			}
			return false;
		}

		internal static Type[] GetTypes(object[] parameters) {
			if (parameters == null) {
				return null;
			}
			if (parameters.Length <= 0) {
				return new Type[0];
			}
			var types = new List<Type>(parameters.Length);
			Array.ForEach(parameters, x => types.Add(x.GetType()));
			return types.ToArray();
		}

		/// <summary>
		/// 在当前应用程序域中查找指定的类型
		/// </summary>
		/// <param name="typeName">类型全名（包括命名空间）</param>
		/// <returns>找到则返回指定的类型，否则返回空</returns>
		public static Type FindType(string typeName) {
			Type type = null;
			var assemblies = LoadAssembliesFromBin();
			foreach (var assembly in assemblies) {
				type = assembly.GetType(typeName, false);
				if (type != null) {
					break;
				}
			}
			return type;
		}

		/// <summary>
		/// 从程序集中获得元属性
		/// </summary>
		/// <param name="assemblies">程序集，如果为null，则从当前应用程序域中获取所载入的所有程序集</param>
		/// <returns>找到的元属性的数组</returns>
		public static T[] GetAttributeFromAssembly<T>(Assembly[] assemblies) where T : Attribute {
			return GetAttributeFromAssembly<T>(assemblies, null);
		}

		/// <summary>
		/// 从程序集中获得元属性
		/// </summary>
		/// <param name="assemblies">程序集，如果为null，则从当前应用程序域中获取所载入的所有程序集</param>
		/// <param name="initializer">初始化元属性</param>
		/// <returns>找到的元属性的数组</returns>
		public static T[] GetAttributeFromAssembly<T>(Assembly[] assemblies, Action<T[], Assembly> initializer) where T : Attribute {
			var list = new List<T>();
			if (assemblies == null) {
				assemblies = LoadAssembliesFromBin();
			}
			foreach (var assembly in assemblies) {
				var attributes = (T[])assembly.GetCustomAttributes(typeof(T), false);
				if (attributes.Length > 0) {
					if (initializer != null) {
						initializer(attributes, assembly);
					}
					list.AddRange(attributes);
				}
			}
			return list.ToArray();
		}

		/// <summary>
		/// 通过反射设置属性/字段值
		/// </summary>
		/// <param name="obj">类型实例</param>
		/// <param name="propertyName">属性名</param>
		/// <param name="newValue">属性值</param>
		public static void SetPropertyValue(object obj, string propertyName, object newValue) {
			if (obj == null) {
				return;
			}
			SetPropertyValueInternal(obj, propertyName, newValue);
		}

		private static void SetPropertyValueInternal(object obj, string propertyName, object newValue) {
			Type type;
			var flags = DefaultFieldBindingFlags;
			if (obj is Type) {
				type = (Type)obj;
				flags |= BindingFlags.Static;
			} else {
				type = obj.GetType();
			}
			var value = newValue;
			try {
				var property = type.GetProperty(propertyName, flags);
				if (property != null && property.CanWrite) {
					property.SetValue(obj, value, null);
				} else {
					var field = type.GetField(propertyName, flags);
					if (field != null) {
						field.SetValue(obj, value);
					}
				}
			} catch (Exception e) {
				throw new ExceptionBase(-1, string.Format("SetPropertyValue Error: {0}\r\nPropertyName={1}, Value={2}, ValueType={3}", e.Message, propertyName, value, value == null ? null : value.GetType().Name), e);
			}
		}

		private static readonly object loadAssembliesFromBinLockObject = new object();
		private static bool loadAssembliesFromBinInitialized;
		private static Assembly[] LoadAssembliesFromBin() {
			if(!loadAssembliesFromBinInitialized) {
				lock(loadAssembliesFromBinLockObject) {
					if(!loadAssembliesFromBinInitialized) {
						var files = new List<string>();
						foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies()) {
							if (!assembly.GlobalAssemblyCache) {
								files.Add(assembly.ManifestModule.ScopeName.ToLower());
							}
						}
						var path = AppDomain.CurrentDomain.RelativeSearchPath;
						if (string.IsNullOrEmpty(path)) {
							path = AppDomain.CurrentDomain.BaseDirectory;
						}
						var fileNames = Directory.GetFiles(path, "*.dll", SearchOption.TopDirectoryOnly);
						foreach (var file in fileNames) {
							var fileName = Path.GetFileName(file);
							if(fileName != null) {
								if (!files.Contains(fileName.ToLower())) {
									try {
										Assembly.LoadFrom(file);
									} catch (BadImageFormatException) { }
								}
							}
						}
						loadAssembliesFromBinInitialized = true;
					}
				}
			}
			return AppDomain.CurrentDomain.GetAssemblies();
		}
	}
}

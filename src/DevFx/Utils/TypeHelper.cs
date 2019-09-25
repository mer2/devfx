/* Copyright(c) 2005-2011 R2@DevFx.NET, License(LGPL) */

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using DevFx.Reflection;
using Microsoft.Extensions.DependencyModel;

namespace DevFx.Utils
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
		}
		/*
		static object ConstructorInvoke(ConstructorInfo ctor, object[] parameterValues) {
			return ctor.Invoke(parameterValues);
		}
		static object MethodInvoke(MethodInfo method, object instance, object[] parameterValues) {
			return method.Invoke(instance, parameterValues);
		}*/

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
					throw new Exception($"将要创建的类型：{type.FullName}，不是期望的类型：{expectedType.FullName}");
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
				try {
					if (assembly.IsDefined(attributeType, false)) {
						list.Add(assembly);
					}
				} catch {}
			}
			return list.ToArray();
		}

		/// <summary>
		/// 获取当前运行时载入的程序集列表
		/// </summary>
		/// <returns></returns>
		public static Assembly[] GetAllAssemblies() {
			return LoadAssembliesFromBin();
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
					throw new ArgumentNullException(nameof(type));
				}
				return null;
			}
			if(expectedType != null && !expectedType.IsAssignableFrom(type)) {
				if (throwOnError) {
					throw new Exception($"将要创建的类型：{type.FullName}，不是期望的类型：{expectedType.FullName}");
				}
				return null;
			}
			object createdObject = null;
			if (parameterTypes == null) {
				parameterTypes = new Type[] { };
			}
			var constructor = type.GetConstructor(BindingFlags.CreateInstance | BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance, null, parameterTypes, null);
			if (constructor == null) {
				try {
					createdObject = Activator.CreateInstance(type, BindingFlags.CreateInstance | BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance, null, parameterValues, null);
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
			TryInvoke(instance, methodName, out var returnValue, true, parameters);
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
			if (instance is Type type1) {
				type = type1;
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
					initializer?.Invoke(attributes, assembly);
					list.AddRange(attributes);
				}
			}
			return list.ToArray();
		}

		public static List<Assembly> FindAssemblyChildren(Assembly theAssembly = null, List<Assembly> all = null) {
			if(theAssembly == null) {
				theAssembly = typeof(TypeHelper).Assembly;
			}
			var children = new List<Assembly> { theAssembly };
			if(all == null) {
				all = new List<Assembly>(GetAllAssemblies());
			}
			all.Remove(theAssembly);
			FindAssemblyChildren(theAssembly, children, all);
			return children;
		}

		internal static void FindAssemblyChildren(Assembly theAssembly, List<Assembly> children, IEnumerable<Assembly> all) {
			var theName = theAssembly.FullName;
			var theChildren = all.Where(x => x.GetReferencedAssemblies().Any(y => y.FullName == theName));
			if(!theChildren.Any()) {
				return;
			}
			children.AddRange(theChildren);//把引用者添加到列表中
			all = all.Except(theChildren);//在所有中去除引用者
			if(!all.Any()) {
				return;
			}
			foreach(var child in theChildren) {//继续寻找孩子的孩子
				FindAssemblyChildren(child, children, all);
			}
		}

		internal static bool IsAssemblyDebugBuild(ICustomAttributeProvider assembly) {
			return assembly.GetCustomAttributes(false).OfType<DebuggableAttribute>().Select(da => da.IsJITTrackingEnabled).FirstOrDefault();
		}
		private static void LoadAssemblyDependencies(Assembly assembly, IReadOnlyList<CompilationLibrary> libraries, List<string> loadedList) {
			var name = assembly.GetName().Name;
			var library = libraries.FirstOrDefault(x => x.Name == name);
			if (library != null) {
				foreach (var dependency in library.Dependencies) {
					var depsName = dependency.Name;
					if (loadedList.Contains(depsName)) {
						continue;
					}
					loadedList.Add(depsName);
					try {
						var deps = Assembly.Load(depsName);
						if(deps != null) {
							LoadAssemblyDependencies(deps, libraries, loadedList);
						}
					} catch { }
				}
			}
		}
		private static readonly object loadAssembliesFromBinLockObject = new object();
		private static bool loadAssembliesFromBinInitialized;
		private static Assembly[] LoadAssembliesFromBin() {
			if(!loadAssembliesFromBinInitialized) {
				lock(loadAssembliesFromBinLockObject) {
					if(!loadAssembliesFromBinInitialized) {
						var loadedList = new List<string> { typeof(TypeHelper).Assembly.GetName().Name };//处理依赖
						var files = new List<string>();//处理在bin目录里的dll
						foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies()) {
							loadedList.Add(assembly.GetName().Name);
							if (!assembly.GlobalAssemblyCache) {
								files.Add(assembly.ManifestModule.ScopeName.ToLower());
							}
						}

						var entryAssembly = Assembly.GetEntryAssembly();
						var dependencyContext = DependencyContext.Load(entryAssembly);
						var libraries = dependencyContext.CompileLibraries;
						loadedList.Add(entryAssembly.GetName().Name);
						LoadAssemblyDependencies(entryAssembly, libraries, loadedList);

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

		/// <summary>
		/// 对Assembly进行依赖排序
		/// </summary>
		/// <param name="list">排序后的结果</param>
		/// <param name="assemblies">需要进行排序的Assembly</param>
		/// <returns>排序后的结果</returns>
		internal static List<Assembly> SortAssemblies(List<Assembly> list, IEnumerable<Assembly> assemblies) {
			//根据程序集逻辑依赖进行排序
			foreach (var assembly in assemblies) {
				if (!list.Contains(assembly)) {
					list.Add(assembly);
				}
				var index = list.IndexOf(assembly);
				var assemblyNames = assembly.GetReferencedAssemblies();
				var referenced = new List<Assembly>();
				foreach (var assemblyName in assemblyNames) {
					var ass = assemblies.FirstOrDefault(x => x.FullName == assemblyName.FullName);
					if (ass == null) {
						continue;
					}
					var assIndex = list.IndexOf(ass);
					if (assIndex > index) {
						list.Remove(ass);
					}
					if (!list.Contains(ass) && !referenced.Contains(ass)) {
						referenced.Add(ass);
					}
				}
				if (referenced.Count > 0) {
					list.InsertRange(index, referenced);
				}
			}
			//根据程序集用户指定的依赖关系排序
			var sortedList = new List<Assembly>();
			for (var i = 0; i < list.Count;) {
				var assembly = list[i];
				var hasDependency = false;
				if (sortedList.Contains(assembly)) {//已经排序过了，直接跳过
					goto CONTINUE;
				}
				if (!assembly.IsDefined(typeof(AssemblyDependencyAttribute), false)) {//没有此属性，直接跳过
					goto CONTINUE;
				}
				var attributes = assembly.GetCustomAttributes(typeof(AssemblyDependencyAttribute), false) as AssemblyDependencyAttribute[];
				if (attributes == null || attributes.Length <= 0) {//没有此属性，直接跳过
					goto CONTINUE;
				}
				//获取依赖属性
				var sortedAttributes = new List<AssemblyDependencyAttribute>(attributes);
				//排序
				sortedAttributes.Sort((x, y) => x.Index - y.Index);
				sortedAttributes.Reverse();
				//把依赖程序集放到本程序集前
				foreach (var dependencyAttribute in sortedAttributes) {
					var assName = dependencyAttribute.AssemblyName;
					if (string.IsNullOrEmpty(assName)) {
						continue;
					}
					var dependencyAssembly = AssemblyLoad(assName, false);
					if (dependencyAssembly == null) {
						continue;
					}
					var dependencyIndex = list.IndexOf(dependencyAssembly);
					if (dependencyIndex <= i) {//不存在或已经在本程序集前了，不处理
						continue;
					}
					//把依赖程序集放到本程序集前
					list.Remove(dependencyAssembly);
					list.Insert(i, dependencyAssembly);
					hasDependency = true;
				}
			CONTINUE:
				if (!hasDependency) {//没有依赖，则移动处理下一个
					i++;
				}
				if (!sortedList.Contains(assembly)) {//把已处理过的标记下，下次就不用再处理了
					sortedList.Add(assembly);
				}
			}
			return list;
		}
	}
}

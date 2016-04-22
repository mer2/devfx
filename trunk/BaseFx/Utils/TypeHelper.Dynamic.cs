/* Copyright(c) 2005-2011 R2@DevFx.NET, License(LGPL) */

using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using System.Threading;
using HTB.DevFx.Reflection;

namespace HTB.DevFx.Utils
{
	partial class TypeHelper
	{
		private static readonly object dynamicLockObject = new object();
		private static volatile Dictionary<string, Type> dynamicTypes;
		private static ModuleBuilder moduleBuilder;

		/// <summary>
		/// 创建动态类型
		/// </summary>
		/// <param name="contractType">需要被实现或复制的接口类型</param>
		/// <param name="baseType">动态类型的基类，必须实现<see cref="IObjectProxyBase{TContract}"/></param>
		/// <param name="implementContract">是否需要实现接口类型</param>
		/// <param name="methodReturnType">方法返回类型</param>
		/// <returns>动态类型</returns>
		public static Type CreateDynamic(Type contractType, Type baseType = null, bool implementContract = true, Type methodReturnType = null) {
			if(contractType == null) {
				throw new ArgumentNullException("contractType");
			}
			if(!contractType.IsInterface) {
				throw new ArgumentException("contractType must be an interface", "contractType");
			}
			if(baseType == null) {
				baseType = typeof(ObjectProxyBase<>).MakeGenericType(contractType);
			}
			const string spaceName = "HTB.DevFx.Remoting.Reflection.DynamicProxies";
			if(dynamicTypes == null) {
				lock (dynamicLockObject) {
					if(dynamicTypes == null) {
						var domain = Thread.GetDomain();
						var name = new AssemblyName(spaceName);
						var builder = domain.DefineDynamicAssembly(name, AssemblyBuilderAccess.Run);
						moduleBuilder = builder.DefineDynamicModule("DynamicModule");
						dynamicTypes = new Dictionary<string, Type>();
					}
				}
			}
			var key = spaceName + "." + contractType.FullName + "." + baseType.FullName;
			if(methodReturnType != null) {
				key += "." + methodReturnType.FullName;
			}
			Type type;
			if(!dynamicTypes.TryGetValue(key, out type)) {
				lock (dynamicTypes) {
					if(!dynamicTypes.TryGetValue(key, out type)) {
						var typeBuilder = CreateDynamic(moduleBuilder, contractType, baseType, implementContract, methodReturnType);
						type = typeBuilder.CreateType();
						dynamicTypes.Add(key, type);
					}
				}
			}
			return type;
		}
		
		private static TypeBuilder CreateDynamic(ModuleBuilder moduleBuilder, Type contractType, Type baseType, bool implementContract, Type methodReturnType) {
			var contextType = typeof(CallContext);
			var proxyType = typeof (IObjectProxyBase<>).MakeGenericType(contractType);
			var interfaces = implementContract ? new[] {contractType} : new Type[] {};
			var typeBuilder = moduleBuilder.DefineType(moduleBuilder.Assembly.GetName().Name + "." + contractType.FullName, TypeAttributes.Public | TypeAttributes.Class, baseType, interfaces);
			var ctor = typeBuilder.DefineMethod(".ctor", MethodAttributes.Public | MethodAttributes.SpecialName | MethodAttributes.RTSpecialName, typeof(void), new[] { contractType });
			var ctorBase = baseType.GetConstructor(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, new Type[] { }, null);
			var setProxyInstance = proxyType.GetMethod("set_ProxyInstance", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, new[] { contractType }, null);
			var getProxyInstance = proxyType.GetMethod("get_ProxyInstance", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, new Type[] { }, null);
			var beforeCall = proxyType.GetMethod("BeforeCall", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, new[] { contextType }, null);
			var afterCall = proxyType.GetMethod("AfterCall", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, new[] { contextType }, null);
			var getCallContext = proxyType.GetMethod("GetCallContext", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, new[] { typeof(MethodInfo), typeof(object[]) }, null);
			var getType = typeof(Type).GetMethod("GetTypeFromHandle", BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic, null, new[] { typeof(RuntimeTypeHandle) }, null);
			var getMethod = typeof(Type).GetMethod("GetMethod", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, new[] { typeof(String), typeof(Type[]) }, null);
			var getResultInitialized = contextType.GetMethod("get_ResultInitialized", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, new Type[] { }, null);
			var setResultInitialized = contextType.GetMethod("set_ResultInitialized", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, new[] { typeof(Boolean) }, null);
			var getResultValue = contextType.GetMethod("get_ResultValue", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, new Type[] { }, null);
			var setResultValue = contextType.GetMethod("set_ResultValue", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, new[] { typeof(Object) }, null);
			var setError = contextType.GetMethod("set_Error", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, new[] { typeof(Exception) }, null);
			var onException = proxyType.GetMethod("OnException", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, new[] { typeof(CallContext) }, null);

			var gen = ctor.GetILGenerator();
			gen.DeclareLocal(proxyType);	//0:var IObjectProxyBase<>
			gen.Emit(OpCodes.Ldarg_0);
			gen.Emit(OpCodes.Call, ctorBase);
			gen.Emit(OpCodes.Ldarg_0);
			gen.Emit(OpCodes.Stloc_0);
			gen.Emit(OpCodes.Ldloc_0);
			gen.Emit(OpCodes.Ldarg_1);
			gen.Emit(OpCodes.Call, setProxyInstance);
			gen.Emit(OpCodes.Ret);
			var methods = contractType.GetMethods();
			foreach(var methodInfo in methods) {
				var parameters = methodInfo.GetParameters();
				var resultType = methodReturnType ?? methodInfo.ReturnType;
				var hasReturnValue = resultType != typeof(void);
				var list = new List<Type>();
				foreach(var parameterInfo in parameters) {
					list.Add(parameterInfo.ParameterType);
				}
				var methodBuilder = typeBuilder.DefineMethod(methodInfo.Name, MethodAttributes.Public | MethodAttributes.Virtual | MethodAttributes.NewSlot, CallingConventions.Standard, resultType, list.ToArray());
				for(var i = 0; i < parameters.Length; i++) {
					var parameterInfo = parameters[i];
					methodBuilder.DefineParameter(i + 1, ParameterAttributes.None, parameterInfo.Name);
				}

				gen = methodBuilder.GetILGenerator();

				// Preparing locals
				gen.DeclareLocal(typeof(MethodInfo));	//0:var MethodInfo
				gen.DeclareLocal(contextType);			//1:var CallContext
				gen.DeclareLocal(typeof(Type[]));		//2:var Type[]
				gen.DeclareLocal(typeof(Object[]));		//3:var Object[]
				gen.DeclareLocal(typeof(Boolean));		//4:var Boolean
				gen.DeclareLocal(typeof(Exception));	//5:var Exception
				gen.DeclareLocal(proxyType);			//6:var IObjectProxyBase<>

				// Preparing labels
				var labelExit = gen.DefineLabel();

				// Writing body

				//Cast this as IObjectProxyBase<>
				gen.Emit(OpCodes.Ldarg_0);
				gen.Emit(OpCodes.Stloc, 6);

				//typeof(TContract)
				gen.Emit(OpCodes.Ldtoken, contractType);
				gen.Emit(OpCodes.Call, getType);

				//GetMothed(...)
				gen.Emit(OpCodes.Ldstr, methodInfo.Name);
				gen.Emit(OpCodes.Ldc_I4, parameters.Length);
				gen.Emit(OpCodes.Newarr, typeof(Type));
				gen.Emit(OpCodes.Stloc_2);
				for(var i = 0; i < parameters.Length; i++) {
					gen.Emit(OpCodes.Ldloc_2);
					gen.Emit(OpCodes.Ldc_I4, i);
					gen.Emit(OpCodes.Ldtoken, parameters[i].ParameterType);
					gen.Emit(OpCodes.Call, getType);
					gen.Emit(OpCodes.Stelem_Ref);
				}
				gen.Emit(OpCodes.Ldloc_2);
				gen.Emit(OpCodes.Call, getMethod);
				gen.Emit(OpCodes.Stloc_0);

				//GetCallContext(...)
				gen.Emit(OpCodes.Ldloc, 6);
				gen.Emit(OpCodes.Ldloc_0);
				gen.Emit(OpCodes.Ldc_I4, parameters.Length);
				gen.Emit(OpCodes.Newarr, typeof(object));
				gen.Emit(OpCodes.Stloc_3);
				for(var i = 0; i < parameters.Length; i++) {
					gen.Emit(OpCodes.Ldloc_3);
					gen.Emit(OpCodes.Ldc_I4, i);
					gen.Emit(OpCodes.Ldarg, i + 1);
					gen.Emit(OpCodes.Stelem_Ref);
				}
				gen.Emit(OpCodes.Ldloc_3);
				gen.Emit(OpCodes.Call, getCallContext);
				gen.Emit(OpCodes.Stloc_1);

				//beforeCall(ctx)
				gen.Emit(OpCodes.Ldloc, 6);
				gen.Emit(OpCodes.Ldloc_1);
				gen.Emit(OpCodes.Call, beforeCall);

				//if(ctx.ResultInitialized)
				gen.Emit(OpCodes.Ldloc_1);
				gen.Emit(OpCodes.Callvirt, getResultInitialized);
				gen.Emit(OpCodes.Stloc_S, 4);
				gen.Emit(OpCodes.Ldloc_S, 4);
				gen.Emit(OpCodes.Brtrue_S, labelExit);

				//try { ...
				gen.BeginExceptionBlock();
				//Call TContract Method
				if(hasReturnValue) {
					gen.Emit(OpCodes.Ldloc_1);
				}
				gen.Emit(OpCodes.Ldloc, 6);
				gen.Emit(OpCodes.Call, getProxyInstance);
				for(var i = 0; i < parameters.Length; i++) {
					gen.Emit(OpCodes.Ldarg, i + 1);
				}
				gen.Emit(OpCodes.Call, methodInfo);
				if(hasReturnValue) {
					gen.Emit(OpCodes.Callvirt, setResultValue);
				}

				//ctx.ResultInitialized = true
				gen.Emit(OpCodes.Ldloc_1);
				gen.Emit(OpCodes.Ldc_I4_1);
				gen.Emit(OpCodes.Callvirt, setResultInitialized);

				// try }
				gen.Emit(OpCodes.Leave_S, labelExit);

				//catch(Exception) { ...
				gen.BeginCatchBlock(typeof(Exception));
				//ctx.Error = e
				gen.Emit(OpCodes.Stloc, 5);
				gen.Emit(OpCodes.Ldloc_1);
				gen.Emit(OpCodes.Ldloc, 5);
				gen.Emit(OpCodes.Callvirt, setError);

				//this.OnException(e)
				gen.Emit(OpCodes.Ldloc, 6);
				gen.Emit(OpCodes.Ldloc_1);
				gen.Emit(OpCodes.Call, onException);
				gen.Emit(OpCodes.Leave_S, labelExit);

				gen.EndExceptionBlock();
				gen.MarkLabel(labelExit);

				//afterCall(ctx)
				gen.Emit(OpCodes.Ldloc, 6);
				gen.Emit(OpCodes.Ldloc_1);
				gen.Emit(OpCodes.Call, afterCall);

				if(hasReturnValue) {
					gen.Emit(OpCodes.Ldloc_1);
					gen.Emit(OpCodes.Callvirt, getResultValue);
				}
				gen.Emit(OpCodes.Ret);
			}
			return typeBuilder;
		}
	}
}

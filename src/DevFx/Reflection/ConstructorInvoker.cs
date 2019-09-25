/* Copyright(c) 2005-2011 R2@DevFx.NET, License(LGPL) */

using System;
using System.Collections.Generic;
using System.Reflection;
using System.Linq.Expressions;

namespace DevFx.Reflection
{
	internal class ConstructorInvoker : IConstructorInvoker
	{
		public ConstructorInvoker(ConstructorInfo constructorInfo) {
			this.ConstructorInfo = constructorInfo;
			this.invoker = InitializeInvoker(constructorInfo);
		}

		public ConstructorInfo ConstructorInfo { get; }

		private readonly Func<object[], object> invoker;
		private static Func<object[], object> InitializeInvoker(ConstructorInfo constructorInfo) {
			// Target: (object)new T((T0)parameters[0], (T1)parameters[1], ...)

			// parameters to execute
			var parametersParameter = Expression.Parameter(typeof(object[]), "parameters");

			// build parameter list
			var parameterExpressions = new List<Expression>();
			var paramInfos = constructorInfo.GetParameters();
			for (var i = 0; i < paramInfos.Length; i++) {
				// (Ti)parameters[i]
				var valueObj = Expression.ArrayIndex(parametersParameter, Expression.Constant(i));
				var valueCast = Expression.Convert(valueObj, paramInfos[i].ParameterType);

				parameterExpressions.Add(valueCast);
			}

			// new T((T0)parameters[0], (T1)parameters[1], ...)
			var instanceCreate = Expression.New(constructorInfo, parameterExpressions);

			// (object)new T((T0)parameters[0], (T1)parameters[1], ...)
			var instanceCreateCast = Expression.Convert(instanceCreate, typeof(object));

			var lambda = Expression.Lambda<Func<object[], object>>(instanceCreateCast, parametersParameter);

			return lambda.Compile();
		}

		public object Invoke(params object[] parameters) {
			return this.invoker(parameters);
		}

		#region IConstructorInvoker Members

		object IConstructorInvoker.Invoke(params object[] parameters) {
			return this.Invoke(parameters);
		}

		#endregion
	}
}
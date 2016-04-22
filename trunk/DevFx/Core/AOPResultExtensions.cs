/* Copyright(c) 2005-2011 R2@DevFx.NET, License(LGPL) */

using System;
using HTB.DevFx.Exceptions;

namespace HTB.DevFx.Core
{
	/// <summary>
	/// <see cref="IAOPResult"/> 相关的扩展方法
	/// </summary>
	public static class AOPResultExtensions
	{
		/// <summary>
		/// 处理结果是否成功（ResultNo == 0）
		/// </summary>
		public static bool IsSuccess(this IAOPResult aop) {
			return aop.ResultNo == 0;
		}

		/// <summary>
		/// 处理结果是否失败（ResultNo != 0 ）
		/// </summary>
		public static bool IsUnSuccess(this IAOPResult aop) {
			return aop.ResultNo != 0;
		}

		/// <summary>
		/// 处理结果是否失败（ResultNo &lt; 0 ）
		/// </summary>
		public static bool IsFailed(this IAOPResult aop) {
			return aop.ResultNo < 0;
		}

		/// <summary>
		/// 已处理，但有不致命的错误（ResultNo &gt; 0）
		/// </summary>
		public static bool IsPassedButFailed(this IAOPResult aop) {
			return aop.ResultNo > 0;
		}

		/// <summary>
		/// 如果处理失败，则抛出异常 <see cref="ExceptionBase"/>
		/// </summary>
		/// <returns>返回本身</returns>
		public static IAOPResult ThrowErrorOnFailed(this IAOPResult aop) {
			if (aop.IsFailed()) {
				var e = new ExceptionBase(aop.ResultNo, aop.ResultDescription);
				e.Data.Add(typeof(IAOPResult), aop);
				throw e;
			}
			return aop;
		}

		/// <summary>
		/// 如果处理失败，则抛出异常 <see cref="ExceptionBase"/>
		/// </summary>
		/// <returns>返回本身</returns>
		public static IAOPResult<T> ThrowErrorOnFailed<T>(this IAOPResult<T> aop) {
			((IAOPResult)aop).ThrowErrorOnFailed();
			return aop;
		}

		/// <summary>
		/// 获取附加对象
		/// </summary>
		/// <param name="aop"><see cref="IAOPResult"/> 泛型实例</param>
		/// <param name="throwErrorOnFailed">失败时是否抛出异常</param>
		/// <returns>附加对象</returns>
		public static T GetAttachObject<T>(this IAOPResult aop, bool throwErrorOnFailed) {
			if (throwErrorOnFailed) {
				aop.ThrowErrorOnFailed();
			}
			var result = aop.ResultAttachObject;
			if(result != null && result is T) {
				return (T)result;
			}
			return default(T);
		}

		/// <summary>
		/// 获取附加对象
		/// </summary>
		/// <param name="aop"><see cref="IAOPResult"/> 泛型实例</param>
		/// <param name="throwErrorOnFailed">失败时是否抛出异常</param>
		/// <returns>附加对象</returns>
		public static T GetAttachObject<T>(this IAOPResult<T> aop, bool throwErrorOnFailed) {
			if (throwErrorOnFailed) {
				aop.ThrowErrorOnFailed();
			}
			return aop.ResultAttachObjectEx;
		}

		/// <summary>
		/// 对返回结果进行处理
		/// </summary>
		/// <param name="aop"><see cref="IAOPResult"/>实例</param>
		/// <param name="handler">处理委托</param>
		/// <returns><see cref="IAOPResult"/>实例</returns>
		public static IAOPResult Handle(this IAOPResult aop, Func<IAOPResult, IAOPResult> handler) {
			if (handler != null) {
				aop = handler(aop);
			}
			return aop;
		}

		/// <summary>
		/// 对返回结果进行处理
		/// </summary>
		/// <param name="aop"><see cref="IAOPResult{T}"/>实例</param>
		/// <param name="handler">处理委托</param>
		/// <returns><see cref="IAOPResult{T}"/>实例</returns>
		public static IAOPResult<T> Handle<T>(this IAOPResult<T> aop, Func<IAOPResult<T>, IAOPResult<T>> handler) {
			if (handler != null) {
				aop = handler(aop);
			}
			return aop;
		}

		/// <summary>
		/// 对成功结果进行处理
		/// </summary>
		/// <param name="aop"><see cref="IAOPResult"/>实例</param>
		/// <param name="handler">处理委托</param>
		/// <returns><see cref="IAOPResult"/>实例</returns>
		public static IAOPResult Success(this IAOPResult aop, Func<IAOPResult, IAOPResult> handler) {
			if (aop.IsSuccess() && handler != null) {
				aop = handler(aop);
			}
			return aop;
		}

		/// <summary>
		/// 对成功结果进行处理
		/// </summary>
		/// <typeparam name="T">泛型类型</typeparam>
		/// <param name="aop"><see cref="IAOPResult{T}"/>实例</param>
		/// <param name="handler">处理委托</param>
		/// <returns><see cref="IAOPResult{T}"/>实例</returns>
		public static IAOPResult<T> Success<T>(this IAOPResult<T> aop, Func<IAOPResult<T>, IAOPResult<T>> handler) {
			if (aop.IsSuccess() && handler != null) {
				aop = handler(aop);
			}
			return aop;
		}

		/// <summary>
		/// 对成功结果进行处理
		/// </summary>
		/// <param name="aop"><see cref="IAOPResult"/>实例</param>
		/// <param name="handler">处理委托</param>
		/// <returns><see cref="IAOPResult"/>实例</returns>
		public static IAOPResult Success(this IAOPResult aop, Action<IAOPResult> handler) {
			if (aop.IsSuccess() && handler != null) {
				handler(aop);
			}
			return aop;
		}

		/// <summary>
		/// 对成功结果进行处理
		/// </summary>
		/// <typeparam name="T">泛型类型</typeparam>
		/// <param name="aop"><see cref="IAOPResult{T}"/>实例</param>
		/// <param name="handler">处理委托</param>
		/// <returns><see cref="IAOPResult{T}"/>实例</returns>
		public static IAOPResult<T> Success<T>(this IAOPResult<T> aop, Action<IAOPResult<T>> handler) {
			if (aop.IsSuccess() && handler != null) {
				handler(aop);
			}
			return aop;
		}

		/// <summary>
		/// 对失败结果进行处理
		/// </summary>
		/// <param name="aop"><see cref="IAOPResult{T}"/>实例</param>
		/// <param name="handler">处理委托</param>
		/// <returns><see cref="IAOPResult"/>实例</returns>
		public static IAOPResult Failed(this IAOPResult aop, Func<IAOPResult, IAOPResult> handler) {
			if (aop.IsUnSuccess() && handler != null) {
				aop = handler(aop);
			}
			return aop;
		}

		/// <summary>
		/// 对失败结果进行处理
		/// </summary>
		/// <typeparam name="T">泛型类型</typeparam>
		/// <param name="aop"><see cref="IAOPResult{T}"/>实例</param>
		/// <param name="handler">处理委托</param>
		/// <returns><see cref="IAOPResult{T}"/>实例</returns>
		public static IAOPResult<T> Failed<T>(this IAOPResult<T> aop, Func<IAOPResult<T>, IAOPResult<T>> handler) {
			if (aop.IsUnSuccess() && handler != null) {
				aop = handler(aop);
			}
			return aop;
		}

		/// <summary>
		/// 对失败结果进行处理
		/// </summary>
		/// <param name="aop"><see cref="IAOPResult{T}"/>实例</param>
		/// <param name="handler">处理委托</param>
		/// <returns><see cref="IAOPResult"/>实例</returns>
		public static IAOPResult Failed(this IAOPResult aop, Action<IAOPResult> handler) {
			if (aop.IsUnSuccess() && handler != null) {
				handler(aop);
			}
			return aop;
		}

		/// <summary>
		/// 对失败结果进行处理
		/// </summary>
		/// <typeparam name="T">泛型类型</typeparam>
		/// <param name="aop"><see cref="IAOPResult{T}"/>实例</param>
		/// <param name="handler">处理委托</param>
		/// <returns><see cref="IAOPResult{T}"/>实例</returns>
		public static IAOPResult<T> Failed<T>(this IAOPResult<T> aop, Action<IAOPResult<T>> handler) {
			if (aop.IsUnSuccess() && handler != null) {
				handler(aop);
			}
			return aop;
		}

		/// <summary>
		/// 把<c>IAOPResult&lt;Q&gt;</c>转换成<c>IAOPResult&lt;P&gt;</c>，其中<typeparamref name="Q"/>从<typeparamref name="P"/>派生
		/// </summary>
		/// <typeparam name="Q">附加的泛型</typeparam>
		/// <typeparam name="P">附加的泛型</typeparam>
		/// <param name="result">待转换的<c>IAOPResult&lt;T&gt;</c></param>
		/// <returns>转换后的<c>IAOPResult&lt;T&gt;</c></returns>
		public static IAOPResult<P> CovertTo<Q, P>(this IAOPResult<Q> result) where Q : P {
			return AOPResult.Create<P>(result.ResultNo, result.ResultDescription, result.ResultAttachObjectEx, result, result.GetResultData());
		}

		/// <summary>
		/// 把<c>IAOPResult&lt;Q&gt;</c>转换成<c>IAOPResult&lt;P&gt;</c>
		/// </summary>
		/// <typeparam name="Q">附加的泛型</typeparam>
		/// <typeparam name="P">附加的泛型</typeparam>
		/// <param name="result">待转换的<c>IAOPResult&lt;T&gt;</c></param>
		/// <returns>转换后的<c>IAOPResult&lt;T&gt;</c></returns>
		public static IAOPResult<P> ToAOPResult<Q, P>(this IAOPResult<Q> result) {
			return AOPResult.Create(result.ResultNo, result.ResultDescription, default(P), result, result.GetResultData());
		}

		/// <summary>
		/// 把<c>IAOPResult</c>转换成<c>IAOPResult&lt;T&gt;</c>
		/// </summary>
		/// <typeparam name="T">附加的泛型</typeparam>
		/// <param name="aop">待转换的<c>IAOPResult</c></param>
		/// <returns>转换后的<c>IAOPResult&lt;T&gt;</c></returns>
		public static IAOPResult<T> ToAOPResult<T>(this IAOPResult aop) {
			if(aop is IAOPResult<T>) {
				return (IAOPResult<T>)aop;
			}
			var attachObject = default(T);
			if(aop.ResultAttachObject is T) {
				attachObject = (T)aop.ResultAttachObject;
			}
			return AOPResult.Create(aop.ResultNo, aop.ResultDescription, attachObject, aop, aop.GetResultData());
		}

		/// <summary>
		/// 把<c>IAOPResult</c>转换成<c>IAOPResult&lt;T&gt;</c>
		/// </summary>
		/// <typeparam name="T">附加的泛型</typeparam>
		/// <param name="aop">待转换的<c>IAOPResult</c></param>
		/// <param name="initializer">初始器</param>
		/// <returns>转换后的<c>IAOPResult&lt;T&gt;</c></returns>
		public static IAOPResult<T> ToAOPResult<T>(this IAOPResult aop, Action<AOPResult<T>> initializer) {
			if (aop is IAOPResult<T>) {
				return (IAOPResult<T>)aop;
			}
			var attachObject = default(T);
			if (aop.ResultAttachObject is T) {
				attachObject = (T)aop.ResultAttachObject;
			}
			var result = (AOPResult<T>)AOPResult.Create(aop.ResultNo, aop.ResultDescription, attachObject, aop, aop.GetResultData());
			if(initializer != null) {
				initializer(result);
			}
			return result;
		}

		/// <summary>
		/// 把<see cref="IAOPResult"/>接口转换成类实例<see cref="AOPResult"/>
		/// </summary>
		/// <param name="aop"><see cref="IAOPResult"/>接口实例</param>
		/// <returns><see cref="AOPResult"/>实例</returns>
		public static AOPResult ToAOPResult(this IAOPResult aop) {
			return aop.ToAOPResult(null);
		}

		/// <summary>
		/// 把<see cref="IAOPResult"/>接口转换成类实例<see cref="AOPResult"/>
		/// </summary>
		/// <param name="aop"><see cref="IAOPResult"/>接口实例</param>
		/// <param name="initializer">初始器</param>
		/// <returns><see cref="AOPResult"/>实例</returns>
		public static AOPResult ToAOPResult(this IAOPResult aop, Action<AOPResult> initializer) {
			if (aop != null) {
				var result = aop is AOPResult ? (AOPResult)aop : (AOPResult)AOPResult.Create(aop.ResultNo, aop.ResultDescription, aop.ResultAttachObject, aop.InnerAOPResult, aop.GetResultData());
				if(initializer != null) {
					initializer(result);
				}
				return result;
			}
			return null;
		}

		/// <summary>
		/// 把<see cref="Exception"/>包装成<see cref="IAOPResult"/>
		/// </summary>
		/// <param name="e"><see cref="Exception"/></param>
		/// <returns><see cref="IAOPResult"/></returns>
		public static IAOPResult ToAOPResult(this Exception e) {
			var key = typeof(IAOPResult);
			if(e.Data.Contains(key)) {
				var aop = e.Data[key] as IAOPResult;
				if (aop != null) {
					return aop;
				}
			}
			return AOPResult.Create(-1, e.Message, e, null);
		}

		/// <summary>
		/// 把<see cref="Exception"/>包装成<see cref="IAOPResult{T}"/>
		/// </summary>
		/// <typeparam name="T">泛型类型</typeparam>
		/// <param name="e"><see cref="Exception"/></param>
		/// <returns><see cref="IAOPResult{T}"/></returns>
		public static IAOPResult<T> ToAOPResult<T>(this Exception e) {
			return e.ToAOPResult().ToAOPResult<T>();
		}
	}
}

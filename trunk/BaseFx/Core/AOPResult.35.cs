/* Copyright(c) 2005-2011 R2@DevFx.NET, License(LGPL) */

using System;
using System.Collections;
using HTB.DevFx.Exceptions;

namespace HTB.DevFx.Core
{
	partial class AOPResult
	{
		#region static methods

		/// <summary>
		/// 创建<see cref="IAOPResult"/>实例
		/// </summary>
		/// <param name="resultNo">返回代码</param>
		/// <param name="resultDescription">描述信息</param>
		/// <param name="attachObject">附加信息</param>
		/// <param name="innerAOPResult">内部AOPResult</param>
		/// <returns><see cref="IAOPResult"/>实例</returns>
		public static IAOPResult Create(int resultNo, string resultDescription, object attachObject, IAOPResult innerAOPResult) {
			return new AOPResult(resultNo, resultDescription, attachObject, ToAOPResult(innerAOPResult, null));
		}

		/// <summary>
		/// 创建<see cref="IAOPResult"/>实例
		/// </summary>
		/// <param name="resultNo">返回代码</param>
		/// <param name="resultDescription">描述信息</param>
		/// <param name="attachObject">附加信息</param>
		/// <param name="innerAOPResult">内部AOPResult</param>
		/// <param name="resultData">传递的其他信息集合（上下文）</param>
		/// <returns><see cref="IAOPResult"/>实例</returns>
		public static IAOPResult Create(int resultNo, string resultDescription, object attachObject, IAOPResult innerAOPResult, IDictionary resultData) {
			return new AOPResult(resultNo, resultDescription, attachObject, ToAOPResult(innerAOPResult, null)) { ResultData = resultData };
		}

		/// <summary>
		/// 创建<see cref="IAOPResult{T}"/>实例
		/// </summary>
		/// <typeparam name="T">泛型</typeparam>
		/// <param name="resultNo">返回代码</param>
		/// <param name="resultDescription">描述信息</param>
		/// <param name="attachObject">附加信息</param>
		/// <param name="innerAOPResult">内部AOPResult</param>
		/// <returns><see cref="IAOPResult{T}"/>实例</returns>
		public static IAOPResult<T> Create<T>(int resultNo, string resultDescription, T attachObject, IAOPResult innerAOPResult) {
			return new AOPResult<T>(resultNo, resultDescription, attachObject, ToAOPResult(innerAOPResult, null));
		}

		/// <summary>
		/// 创建<see cref="IAOPResult{T}"/>实例
		/// </summary>
		/// <typeparam name="T">泛型</typeparam>
		/// <param name="resultNo">返回代码</param>
		/// <param name="resultDescription">描述信息</param>
		/// <param name="attachObject">附加信息</param>
		/// <param name="innerAOPResult">内部AOPResult</param>
		/// <param name="resultData">传递的其他信息集合（上下文）</param>
		/// <returns><see cref="IAOPResult{T}"/>实例</returns>
		public static IAOPResult<T> Create<T>(int resultNo, string resultDescription, T attachObject, IAOPResult innerAOPResult, IDictionary resultData) {
			return new AOPResult<T>(resultNo, resultDescription, attachObject, ToAOPResult(innerAOPResult, null)) { ResultData = resultData };
		}

		/// <summary>
		/// 返回执行成功结果
		/// </summary>
		/// <returns>AOPResult</returns>
		public static IAOPResult Success() {
			return new AOPResult();
		}

		/// <summary>
		/// 返回执行成功结果
		/// </summary>
		/// <param name="resultDescription">对应的描述信息</param>
		/// <returns>AOPResult</returns>
		public static IAOPResult Success(string resultDescription) {
			return new AOPResult(0, resultDescription, null, null);
		}

		/// <summary>
		/// 返回执行成功结果（附加对象）
		/// </summary>
		/// <param name="attachObject">附加对象</param>
		/// <returns>IAOPResult（泛型）</returns>
		public static IAOPResult<T> Success<T>(T attachObject) {
			return new AOPResult<T>(0, null, attachObject, null);
		}

		/// <summary>
		/// 返回执行失败结果
		/// </summary>
		/// <param name="resultNo">返回代码</param>
		/// <param name="resultDescription">对应的描述信息</param>
		/// <returns><see cref="IAOPResult"/>实例</returns>
		public static IAOPResult Failed(int resultNo, string resultDescription) {
			return Create(resultNo, resultDescription, null, null);
		}

		/// <summary>
		/// 返回执行失败结果
		/// </summary>
		/// <param name="resultNo">返回代码</param>
		/// <param name="resultDescription">对应的描述信息</param>
		/// <returns><see cref="IAOPResult{T}"/>实例</returns>
		public static IAOPResult<T> Failed<T>(int resultNo, string resultDescription) {
			return Create(resultNo, resultDescription, default(T), null);
		}

		/// <summary>
		/// 返回执行失败结果
		/// </summary>
		/// <param name="resultDescription">对应的描述信息</param>
		/// <returns><see cref="IAOPResult"/>实例</returns>
		public static IAOPResult Failed(string resultDescription) {
			return Failed(-1, resultDescription);
		}

		/// <summary>
		/// 返回执行失败结果
		/// </summary>
		/// <typeparam name="T">附加对象类型</typeparam>
		/// <param name="resultDescription">对应的描述信息</param>
		/// <returns><see cref="IAOPResult{T}"/>实例</returns>
		public static IAOPResult<T> Failed<T>(string resultDescription) {
			return Failed<T>(-1, resultDescription);
		}

		/// <summary>
		/// 包装运行
		/// </summary>
		/// <param name="throwOnError">异常时是否抛出</param>
		/// <param name="handler">处理委托</param>
		/// <returns><see cref="IAOPResult"/>实例</returns>
		public static IAOPResult Wrap(bool throwOnError, Func<IAOPResult> handler) {
			IAOPResult aop;
			try {
				aop = handler();
			} catch (ExceptionBase be) {
				aop = be.Data[typeof(IAOPResult)] as IAOPResult;
				if (aop == null && throwOnError) {
					throw;
				}
			} catch (Exception ex) {
				if (throwOnError) {
					throw;
				}
				aop = ToAOPResult(ex);
			}
			return aop;
		}

		/// <summary>
		/// 包装运行
		/// </summary>
		/// <param name="throwOnError">异常时是否抛出</param>
		/// <param name="handler">处理委托</param>
		/// <returns><see cref="IAOPResult"/>实例</returns>
		public static IAOPResult Wrap(bool throwOnError, Action handler) {
			return Wrap(throwOnError, () => {
				handler();
				return Success();
			});
		}

		/// <summary>
		/// 把<see cref="IAOPResult"/>接口转换成类实例<see cref="AOPResult"/>
		/// </summary>
		/// <param name="aop"><see cref="IAOPResult"/>接口实例</param>
		/// <param name="initializer">初始器</param>
		/// <returns><see cref="AOPResult"/>实例</returns>
		internal static AOPResult ToAOPResult(IAOPResult aop, Action<AOPResult> initializer) {
			if (aop != null) {
				var result = aop is AOPResult ? (AOPResult)aop : (AOPResult)Create(aop.ResultNo, aop.ResultDescription, aop.ResultAttachObject, aop.InnerAOPResult, aop.GetResultData());
				if (initializer != null) {
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
		internal static IAOPResult ToAOPResult(Exception e) {
			var key = typeof(IAOPResult);
			if (e.Data.Contains(key)) {
				var aop = e.Data[key] as IAOPResult;
				if (aop != null) {
					return aop;
				}
			}
			return Create(-1, e.Message, e, null);
		}

		#endregion static methods
	}
}

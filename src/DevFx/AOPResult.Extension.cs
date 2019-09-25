using System;
using System.Collections;

namespace DevFx
{
    public partial class AOPResult
    {
	    #region static methods

	    /// <summary>
	    /// 创建<see cref="IAOPResult"/>实例
	    /// </summary>
	    /// <param name="resultNo">返回代码</param>
	    /// <param name="resultDescription">描述信息</param>
	    /// <returns><see cref="IAOPResult"/>实例</returns>
	    public static IAOPResult Create(int resultNo, string resultDescription) {
		    return new AOPResult(resultNo, resultDescription);
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
	    public static IAOPResult<T> Create<T>(int resultNo, string resultDescription, T attachObject, IAOPResult innerAOPResult = null, IDictionary resultData = null) {
		    return new AOPResult<T>(resultNo, resultDescription, attachObject, ToAOPResult(innerAOPResult, null)) { ResultData = resultData };
	    }

	    /// <summary>
	    /// 返回执行成功结果
	    /// </summary>
	    /// <param name="resultDescription">对应的描述信息</param>
	    /// <returns>AOPResult</returns>
	    public static IAOPResult Success(string resultDescription = null) {
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
		    return Create(resultNo, resultDescription);
	    }

		/// <summary>
		/// 返回执行失败结果
		/// </summary>
		/// <param name="resultDescription">对应的描述信息</param>
		/// <returns><see cref="IAOPResult"/>实例</returns>
		public static IAOPResult Failed(string resultDescription) {
			return Create(-1, resultDescription);
		}

		/// <summary>
		/// 返回执行失败结果
		/// </summary>
		/// <typeparam name="T">附加对象类型</typeparam>
		/// <param name="resultNo">返回代码</param>
		/// <param name="resultDescription">对应的描述信息</param>
		/// <returns><see cref="IAOPResult{T}"/>实例</returns>
		public static IAOPResult<T> Failed<T>(int resultNo, string resultDescription) {
		    return Create<T>(resultNo, resultDescription, default);
	    }

		/// <summary>
		/// 返回执行失败结果
		/// </summary>
		/// <typeparam name="T">附加对象类型</typeparam>
		/// <param name="resultNo">返回代码</param>
		/// <param name="resultDescription">对应的描述信息</param>
		/// <returns><see cref="IAOPResult{T}"/>实例</returns>
		public static IAOPResult<T> Failed<T>(string resultDescription) {
			return Create<T>(-1, resultDescription, default);
		}

		/// <summary>
		/// 把<see cref="IAOPResult"/>接口转换成类实例<see cref="AOPResult"/>
		/// </summary>
		/// <param name="aop"><see cref="IAOPResult"/>接口实例</param>
		/// <param name="initializer">初始器</param>
		/// <returns><see cref="AOPResult"/>实例</returns>
		internal static AOPResult ToAOPResult(IAOPResult aop, Action<AOPResult> initializer) {
		    if(aop != null) {
			    var aopResult = aop as AOPResult ?? (AOPResult)Create(aop.ResultNo, aop.ResultDescription, aop.ResultAttachObject, aop.InnerAOPResult, aop.GetResultData());
			    initializer?.Invoke(aopResult);
			    return aopResult;
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
		    if(e.Data.Contains(key)) {
				if(e.Data[key] is IAOPResult aop) {
					return aop;
				}
			}
		    return new AOPResult(-1, e.Message, e, null);
	    }

	    #endregion static methods
    }
}

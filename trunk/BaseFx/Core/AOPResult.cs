/* Copyright(c) 2005-2011 R2@DevFx.NET, License(LGPL) */

using System;
using System.Runtime.InteropServices;
using HTB.DevFx.Exceptions;

namespace HTB.DevFx.Core
{
	#region AOPResult class

	/// <summary>
	/// 对象处理返回的结果
	/// </summary>
	[Serializable, Guid("CF646C8C-1A90-45BD-990D-08BD6A9DAB8C")]
	public partial class AOPResult : AopResult<object>, IAOPResult
	{
		#region constructor

		/// <summary>
		/// 默认构造函数
		/// </summary>
		public AOPResult() { }

		/// <summary>
		/// 构造函数
		/// </summary>
		/// <param name="resultNo">返回代码</param>
		/// <param name="resultDescription">对应的描述信息</param>
		/// <param name="resultAttachObject">相应的附加信息</param>
		/// <param name="innerAOPResult">内部AOPResult</param>
		public AOPResult(int resultNo, string resultDescription, object resultAttachObject, IAOPResult innerAOPResult) : base(resultNo, resultDescription, resultAttachObject, innerAOPResult) {
		}

		/// <summary>
		/// 构造函数
		/// </summary>
		/// <param name="resultNo">返回代码</param>
		/// <param name="resultDescription">对应的描述信息</param>
		/// <param name="resultAttachObject">相应的附加信息</param>
		/// <param name="innerAOPResult">内部AOPResult</param>
		public AOPResult(int resultNo, string resultDescription, object resultAttachObject, AOPResult innerAOPResult) : base(resultNo, resultDescription, resultAttachObject, innerAOPResult) {
		}

		#endregion
	}

	#endregion AOPResult class

	#region AOPResult<T> class

	/// <summary>
	/// 对象处理返回的结果（泛型）
	/// </summary>
	/// <typeparam name="T"></typeparam>
	[Serializable]
	public class AOPResult<T> : AOPResult, IAOPResult<T>
	{
		#region constructor

		/// <summary>
		/// 默认构造函数
		/// </summary>
		public AOPResult() { }

		/// <summary>
		/// 构造函数
		/// </summary>
		/// <param name="resultNo">返回代码</param>
		/// <param name="resultDescription">对应的描述信息</param>
		/// <param name="resultAttachObject">相应的附加信息</param>
		/// <param name="innerAOPResult">内部AOPResult</param>
		public AOPResult(int resultNo, string resultDescription, T resultAttachObject, AOPResult innerAOPResult) : base(resultNo, resultDescription, resultAttachObject, innerAOPResult) { }

		/// <summary>
		/// 构造函数
		/// </summary>
		/// <param name="resultNo">返回代码</param>
		/// <param name="resultDescription">对应的描述信息</param>
		/// <param name="resultAttachObject">相应的附加信息</param>
		/// <param name="innerAOPResult">内部AOPResult</param>
		public AOPResult(int resultNo, string resultDescription, T resultAttachObject, IAOPResult innerAOPResult) : base(resultNo, resultDescription, resultAttachObject, innerAOPResult) { }

		#endregion constructor

		#region properties

		/// <summary>
		/// 相应的附加信息
		/// </summary>
		public override object ResultAttachObject {
			get { return base.ResultAttachObject; }
			set {
				if(value != null && !(value is T)) {
					//所设置的类型不是所期望的类型
					var type = typeof(T);
					if(type.IsPrimitive && value.GetType().IsPrimitive) {//是基元，尝试转换，看看类型是否兼容
						value = Convert.ChangeType(value, type);
					} else {
						throw new ExceptionBase("所设置的类型不是所期望的类型");
					}
				}
				base.ResultAttachObject = value;
			}
		}

		/// <summary>
		/// 附加信息（泛型）
		/// </summary>
		T IAOPResult<T>.ResultAttachObjectEx {
			get { return (T)base.ResultAttachObject; }
		}

		#endregion properties
	}

	#endregion
}

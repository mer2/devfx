/* Copyright(c) 2005-2011 R2@DevFx.NET, License(LGPL) */

using System;
using System.Collections;
using System.Collections.Specialized;
using System.Runtime.InteropServices;
using HTB.DevFx.Exceptions;

namespace HTB.DevFx.Core
{
	#region AOPResult class

	/// <summary>
	/// 对象处理返回的结果
	/// </summary>
	[Serializable, Guid("CF646C8C-1A90-45BD-990D-08BD6A9DAB8C")]
	public partial class AOPResult : IAOPResult
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
		public AOPResult(int resultNo, string resultDescription, object resultAttachObject, AOPResult innerAOPResult) {
			this.resultNo = resultNo;
			this.resultDescription = resultDescription;
			this.resultAttachObject = resultAttachObject;
			this.innerAOPResult = innerAOPResult;
		}

		#endregion

		#region fields

		private int resultNo;
		private string resultDescription;
		private object resultAttachObject;
		private AOPResult innerAOPResult;
		private bool handled;
		private IDictionary resultData;

		#endregion

		#region properties

		/// <summary>
		/// 返回代码
		/// </summary>
		public virtual int ResultNo {
			get { return this.resultNo; }
			set { this.resultNo = value; }
		}

		/// <summary>
		/// 对应的描述信息
		/// </summary>
		public virtual string ResultDescription {
			get { return this.resultDescription; }
			set { this.resultDescription = value; }
		}

		/// <summary>
		/// 相应的附加信息
		/// </summary>
		public virtual object ResultAttachObject {
			get { return this.resultAttachObject; }
			set { this.resultAttachObject = value; }
		}

		/// <summary>
		/// 内部AOPResult
		/// </summary>
		public virtual AOPResult InnerAOPResult {
			get { return this.innerAOPResult; }
			set { this.innerAOPResult = value; }
		}

		/// <summary>
		/// 是否已处理
		/// </summary>
		public virtual bool Handled {
			get { return this.handled; }
			set { this.handled = value; }
		}

		/// <summary>
		/// 传递的其他信息集合（上下文）
		/// </summary>
		public virtual IDictionary ResultData {
			get { return this.resultData ?? (this.resultData = new HybridDictionary()); }
			set { this.resultData = value; }
		}

		/// <summary>
		/// 获取传递的信息上下文
		/// </summary>
		/// <returns><see cref="IDictionary"/></returns>
		public virtual IDictionary GetResultData() {
			return this.resultData;
		}

		/// <summary>
		/// 转换为可显示信息
		/// </summary>
		/// <returns>可显示信息</returns>
		public override string ToString() {
			return string.Format("{0}/{1}", this.ResultNo, this.ResultDescription);
		}

		#endregion properties

		#region IAOPResult Members

		int IAOPResult.ResultNo {
			get { return this.ResultNo; }
		}

		string IAOPResult.ResultDescription {
			get { return this.ResultDescription; }
		}

		object IAOPResult.ResultAttachObject {
			get { return this.ResultAttachObject; }
		}

		IAOPResult IAOPResult.InnerAOPResult {
			get { return this.InnerAOPResult; }
		}

		bool IAOPResult.Handled {
			get { return this.Handled; }
			set { this.Handled = value; }
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

		#endregion constructor

		#region properties

		/// <summary>
		/// 相应的附加信息
		/// </summary>
		public override object ResultAttachObject {
			get { return base.ResultAttachObject; }
			set {
				if (value != null && !(value is T)) {
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

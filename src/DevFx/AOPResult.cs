/* Copyright(c) 2005-2017 R2@DevFx.NET, License(LGPL) */

using System;
using System.Collections;
using System.Collections.Specialized;

namespace DevFx
{
	#region AOPResult class

	/// <summary>
	/// 对象处理返回的结果
	/// </summary>
	[Serializable]
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
		public AOPResult(int resultNo, string resultDescription, object resultAttachObject = null, IAOPResult innerAOPResult = null) {
			this.ResultNo = resultNo;
			this.ResultDescription = resultDescription;
			this.ResultAttachObject = resultAttachObject;
			this.InnerAOPResult = innerAOPResult;
		}

		#endregion

		#region properties

		/// <summary>
		/// 返回代码
		/// </summary>
		public int ResultNo { get; set; }
		/// <summary>
		/// 对应的描述信息
		/// </summary>
		public string ResultDescription { get; set; }
		/// <summary>
		/// 相应的附加信息
		/// </summary>
		public virtual object ResultAttachObject { get; set; }
		/// <summary>
		/// 内部AOPResult
		/// </summary>
		public virtual IAOPResult InnerAOPResult { get; set; }
		/// <summary>
		/// 是否已处理
		/// </summary>
		public bool Handled { get; set; }

		private IDictionary resultData;
		/// <summary>
		/// 传递的其他信息集合（上下文）
		/// </summary>
		public virtual IDictionary ResultData {
			get => this.resultData ?? (this.resultData = new HybridDictionary());
			set => this.resultData = value;
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
			return $"{this.ResultNo}/{this.ResultDescription}";
		}

		#endregion properties

		#region IAOPResult Members

		int IAOPResult.ResultNo => this.ResultNo;
		string IAOPResult.ResultDescription => this.ResultDescription;
		object IAOPResult.ResultAttachObject => this.ResultAttachObject;
		IAOPResult IAOPResult.InnerAOPResult => this.InnerAOPResult;
		bool IAOPResult.Handled {
			get => this.Handled;
			set => this.Handled = value;
		}

		#endregion
	}

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
		public AOPResult(int resultNo, string resultDescription, T resultAttachObject = default, IAOPResult innerAOPResult = null) : base(resultNo, resultDescription, resultAttachObject, innerAOPResult) {
			this.typedResultObject = resultAttachObject;
		}

		#endregion

		private T typedResultObject;
		public new virtual T ResultAttachObject {
			get => this.typedResultObject;
			set {
				this.typedResultObject = value;
				base.ResultAttachObject = value;
			}
		}
	}

	#endregion AOPResult class
}

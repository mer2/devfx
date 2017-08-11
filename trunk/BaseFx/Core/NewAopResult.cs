using System;
using System.Collections;
using System.Collections.Specialized;

namespace HTB.DevFx.Core
{
	/// <summary>
	/// （新）对象处理返回的结果（泛型）
	/// </summary>
	/// <typeparam name="T"></typeparam>
	[Serializable]
	public class AopResult<T> : IAOPResult<T>
	{
		#region constructor

		/// <summary>
		/// 默认构造函数
		/// </summary>
		public AopResult() { }

		/// <summary>
		/// 构造函数
		/// </summary>
		/// <param name="resultNo">返回代码</param>
		/// <param name="resultDescription">对应的描述信息</param>
		/// <param name="resultAttachObject">相应的附加信息</param>
		/// <param name="innerAOPResult">内部AOPResult</param>
		public AopResult(int resultNo, string resultDescription, T resultAttachObject, IAOPResult innerAOPResult) {
			this.resultNo = resultNo;
			this.resultDescription = resultDescription;
			this.resultAttachObject = resultAttachObject;
			this.innerAOPResult = innerAOPResult;
		}

		#endregion

		#region fields

		private int resultNo;
		private string resultDescription;
		private T resultAttachObject;
		private IAOPResult innerAOPResult;
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
		public virtual T ResultAttachObject {
			get { return this.resultAttachObject; }
			set { this.resultAttachObject = value; }
		}

		/// <summary>
		/// 内部AOPResult
		/// </summary>
		public virtual IAOPResult InnerAOPResult {
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

		T IAOPResult<T>.ResultAttachObjectEx {
			get { return this.ResultAttachObject; }
		}

		#endregion

		/// <summary>
		/// 创建<see cref="IAOPResult{P}"/>实例
		/// </summary>
		/// <typeparam name="P">泛型</typeparam>
		/// <param name="resultNo">返回代码</param>
		/// <param name="resultDescription">描述信息</param>
		/// <param name="attachObject">附加信息</param>
		/// <param name="innerAOPResult">内部AOPResult</param>
		/// <returns><see cref="IAOPResult{P}"/>实例</returns>
		public static IAOPResult<P> Create<P>(int resultNo, string resultDescription, P attachObject = default(P), IAOPResult innerAOPResult = null) {
			return new AopResult<P>(resultNo, resultDescription, attachObject, innerAOPResult);
		}
	}
}

/* Copyright(c) 2005-2011 R2@DevFx.NET, License(LGPL) */

using System.Collections;
using System.Runtime.InteropServices;

namespace HTB.DevFx.Core
{
	#region IAOPResult
	
	/// <summary>
	/// 对象处理返回的结果接口
	/// </summary>
	/// <remarks>
	/// 建议在代码调用返回值中都采用此类实例为返回值<br />
	/// 一般ResultNo小于0表示异常，0表示成功，大于0表示其它一般提示信息
	/// </remarks>
	[Guid("106009C4-859D-4139-8F1D-9799D941662B")]
	public interface IAOPResult
	{
		/// <summary>
		/// 返回代码
		/// </summary>
		int ResultNo { get; }

		/// <summary>
		/// 对应的描述信息
		/// </summary>
		string ResultDescription { get; }

		/// <summary>
		/// 相应的附加信息
		/// </summary>
		object ResultAttachObject { get; }

		/// <summary>
		/// 内部AOPResult
		/// </summary>
		IAOPResult InnerAOPResult { get; }

		/// <summary>
		/// 是否已处理
		/// </summary>
		bool Handled { get; set; }

		/// <summary>
		/// 传递的其他信息集合（上下文）
		/// </summary>
		IDictionary ResultData { get; }

		/// <summary>
		/// 获取传递的信息上下文
		/// </summary>
		/// <returns><see cref="IDictionary"/></returns>
		IDictionary GetResultData();
	}

	#endregion IAOPResult

	#region IAOPResult<T>

	/// <summary>
	/// 对象处理返回的结果接口（泛型）
	/// </summary>
	[Guid("27F931B0-44E2-48E9-BC3F-ED707E68525C")]
	public interface IAOPResult<out T> : IAOPResult
	{
		/// <summary>
		/// 泛型附加对象
		/// </summary>
		T ResultAttachObjectEx { get; }
	}
 
	#endregion
}

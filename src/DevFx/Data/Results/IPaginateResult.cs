/* Copyright(c) 2005-2013 R2@DevFx.NET, License(LGPL) */

namespace DevFx.Data.Results
{
	/// <summary>
	/// 可分页的结果
	/// </summary>
	public interface IPaginateResult
	{
		/// <summary>
		/// 总记录数
		/// </summary>
		int Count { get; }
		/// <summary>
		/// 当前页的记录
		/// </summary>
		object[] Items { get; }
	}

	/// <summary>
	/// 可分页的结果（泛型）
	/// </summary>
	/// <typeparam name="T">分页元素类型</typeparam>
	public interface IPaginateResult<out T> : IPaginateResult
	{
		/// <summary>
		/// 当前页的记录（泛型）
		/// </summary>
		new T[] Items { get; }
	}
}
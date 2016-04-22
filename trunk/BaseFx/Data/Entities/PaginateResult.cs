/* Copyright(c) 2005-2013 R2@DevFx.NET, License(LGPL) */

using System;

namespace HTB.DevFx.Data.Entities
{
	/// <summary>
	/// 可分页的结果
	/// </summary>
	[Serializable]
	public class PaginateResult : IPaginateResult
	{
		/// <summary>
		/// 总记录数
		/// </summary>
		public int Count { get; set; }
		/// <summary>
		/// 当前页的记录
		/// </summary>
		public object[] Items { get; set; }
	}

	/// <summary>
	/// 可分页的结果（泛型）
	/// </summary>
	/// <typeparam name="T">分页元素类型</typeparam>
	[Serializable]
	public class PaginateResult<T> : PaginateResult, IPaginateResult<T>
	{
		private T[] items;

		T[] IPaginateResult<T>.Items {
			get {
				if(this.items == null) {
					this.items = Array.ConvertAll(this.Items, x => (T)x);
				}
				return this.items;
			}
		}
	}
}

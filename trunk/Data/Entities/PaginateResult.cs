/* Copyright(c) 2005-2011 R2@DevFx.NET, License(LGPL) */

using System;

namespace HTB.DevFx.Data.Entities
{
	internal class PaginateResult : IPaginateResult
	{
		public int Count { get; set; }
		public object[] Items { get; set; }
	}

	internal class PaginateResult<T> : PaginateResult, IPaginateResult<T>
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

/* Copyright(c) 2005-2011 R2@DevFx.NET, License(LGPL) */

namespace HTB.DevFx.Data.Entities
{
	public interface IPaginateResult
	{
		int Count { get; }
		object[] Items { get; }
	}

	public interface IPaginateResult<out T> : IPaginateResult
	{
		new T[] Items { get; }
	}
}
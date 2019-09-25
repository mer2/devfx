/* Copyright(c) 2005-2011 R2@DevFx.NET, License(LGPL) */

namespace DevFx.Reflection
{
	internal interface IFastReflectionCache<in TKey, out TValue>
	{
		TValue Get(TKey key);
	}
}
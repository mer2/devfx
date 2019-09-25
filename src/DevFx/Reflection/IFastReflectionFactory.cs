/* Copyright(c) 2005-2011 R2@DevFx.NET, License(LGPL) */

namespace DevFx.Reflection
{
	internal interface IFastReflectionFactory<in TKey, out TValue>
	{
		TValue Create(TKey key);
	}
}
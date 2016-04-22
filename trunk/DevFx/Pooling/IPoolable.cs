/* Copyright(c) 2005-2011 R2@DevFx.NET, License(LGPL) */

using System;

namespace HTB.DevFx.Pooling
{
	public interface IPoolable
	{
		event Action<IPoolable, bool> Disposing;
		void Dispose();
	}
}

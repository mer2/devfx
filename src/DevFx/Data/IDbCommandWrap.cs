/* Copyright(c) 2005-2011 R2@DevFx.NET, License(LGPL) */

using System;
using System.Data.Common;

namespace DevFx.Data
{
	public interface IDbCommandWrap : IDisposable
	{
		DbCommand Command { get; }
	}
}

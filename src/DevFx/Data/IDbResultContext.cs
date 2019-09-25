/* Copyright(c) 2005-2011 R2@DevFx.NET, License(LGPL) */

using DevFx.Core;
using System;

namespace DevFx.Data
{
	public interface IDbResultContext : IObjectContext
	{
		IDbExecuteContext ExecuteContext { get; }
		Type ResultType { get; set; }
		object ResultInstance { get; set; }
		IResultHandler ResultHandler { get; set; }
		bool ResultHandled { get; set; }
	}
}

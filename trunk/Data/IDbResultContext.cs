/* Copyright(c) 2005-2011 R2@DevFx.NET, License(LGPL) */

using System;
using HTB.DevFx.Core;

namespace HTB.DevFx.Data
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

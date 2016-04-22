/* Copyright(c) 2005-2011 R2@DevFx.NET, License(LGPL) */

using System;

namespace HTB.DevFx.Data
{
	public interface IResultHandler
	{
		object ExecuteResult(IDbExecuteContext ctx, Type resultType, object resultInstance);
	}
}
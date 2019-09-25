/* Copyright(c) 2005-2011 R2@DevFx.NET, License(LGPL) */

using DevFx.Core;
using DevFx.Data.Settings;
using System;

namespace DevFx.Data
{
	[Service]
	public interface IResultHandlerFactory
	{
		void Init(IObjectService objectService, IResultHandlerFactoryContextSetting setting);
		object ExecuteResult(IDbExecuteContext ctx, Type resultType, object resultInstance);
	}
}

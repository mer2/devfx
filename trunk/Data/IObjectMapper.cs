/* Copyright(c) 2005-2011 R2@DevFx.NET, License(LGPL) */

using System;
using System.Collections;
using HTB.DevFx.Data.Config;

namespace HTB.DevFx.Data
{
	public interface IObjectMapper
	{
		object Mapping(IStatementSetting statement, IDictionary dataRecord, Type instanceType, object instance);
	}
}

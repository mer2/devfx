/* Copyright(c) 2005-2011 R2@DevFx.NET, License(LGPL) */

using System;
using System.Collections;
using DevFx.Data.Settings;

namespace DevFx.Data
{
	[Service]
	public interface IObjectMapper
	{
		object Mapping(IStatementSetting statement, IDictionary dataRecord, Type instanceType, object instance);
	}
}

/* Copyright(c) 2005-2011 R2@DevFx.NET, License(LGPL) */

using DevFx.Core;
using DevFx.Data.Settings;
using System;
using System.Collections;

namespace DevFx.Data
{
	public interface IDbExecuteContext : IObjectContext, IDisposable
	{
		IDataService DataService { get; }
		IStatementSetting Statement { get; }
		IDataStorage DataStorage { get; }
		IDbCommandWrap CommandWrap { get; }
		IDictionary Parameters { get; }
	}
}

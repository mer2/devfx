/* Copyright(c) 2005-2011 R2@DevFx.NET, License(LGPL) */

using System.Collections;
using HTB.DevFx.Data.Config;

namespace HTB.DevFx.Data
{
	public interface ICommandTextParser
	{
		string GetCommandText(IStatementSetting statement, IDictionary parameters);
	}
}

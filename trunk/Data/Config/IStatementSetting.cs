/* Copyright(c) 2005-2011 R2@DevFx.NET, License(LGPL) */

using System.Data;
using HTB.DevFx.Config;

namespace HTB.DevFx.Data.Config
{
	public interface IStatementSetting : IConfigSettingRequired
	{
		string Name { get; }
		CommandType CommandType { get; }
		int Timeout { get; }
		string ResultTypeName { get; }
		string ResultHandlerName { get; }

		IParameterSetting[] Parameters { get; }

		IStatementTextSetting StatementText { get; }
		string DataStorageName { get; }
		string CommandBuilderTypeName { get; }
	}
}

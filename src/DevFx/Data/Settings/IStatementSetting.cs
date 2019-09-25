/* Copyright(c) 2005-2011 R2@DevFx.NET, License(LGPL) */

using System.Data;

namespace DevFx.Data.Settings
{
	public interface IStatementSetting
	{
		string Name { get; }
		CommandType CommandType { get; }
		int Timeout { get; }
		string ResultTypeName { get; }
		string ResultHandlerName { get; }

		bool AutoParameters { get; }
		IParameterSetting[] Parameters { get; }

		IStatementTextSetting StatementText { get; }
		string DataStorageName { get; }
		string CommandBuilderTypeName { get; }
	}
}

/* Copyright(c) 2005-2011 R2@DevFx.NET, License(LGPL) */

namespace HTB.DevFx.Data.Config
{
	public interface IStatementContextSetting
	{
		string Name { get; }
		string DataStorageName { get; }
		IStatementSetting[] Statements { get; }
	}
}

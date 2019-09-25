/* Copyright(c) 2005-2011 R2@DevFx.NET, License(LGPL) */

namespace DevFx.Data.Settings
{
	public interface IStatementTextSetting
	{
		string CommandText { get; }
		string ParseName { get; }
		
		ICommandTextParser Parser { get; }
	}
}

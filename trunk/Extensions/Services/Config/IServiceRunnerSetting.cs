/* Copyright(c) 2005-2011 R2@DevFx.NET, License(LGPL) */

namespace HTB.DevFx.Services.Config
{
	public interface IServiceRunnerSetting
	{
		string Title { get; }
		string ServiceName { get; }
		bool Enabled { get; }
		string ServiceType { get; }
		string Handler { get; }
	}
}

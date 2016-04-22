/* Copyright(c) 2005-2011 R2@DevFx.NET, License(LGPL) */

namespace HTB.DevFx.Config
{
	public interface IConfigServiceSetting
	{
		string TypeName { get; }
		IConfigDebugSetting Debug { get; }
		IConfigAssemblySetting[] Assemblies { get; }
		IConfigFileSetting[] ConfigFiles { get; }
		IConfigSetting ConfigSetting { get; }
	}

	public interface IConfigAssemblySetting
	{
		string AssemblyName { get; }
	}

	public interface IConfigFileSetting
	{
		string FileName { get; }
	}

	public interface IConfigDebugSetting
	{
		bool Enabled { get; }
		string FileName { get; }
	}
}

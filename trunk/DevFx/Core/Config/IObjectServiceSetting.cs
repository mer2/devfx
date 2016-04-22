/* Copyright(c) 2005-2011 R2@DevFx.NET, License(LGPL) */

namespace HTB.DevFx.Core.Config
{
	public interface IObjectServiceSetting
	{
		string TypeName { get; }
		IObjectServiceExtenderSetting[] Extenders { get; }
		IObjectNamespaceSetting ObjectNamespace { get; }
	}

	public interface IObjectServiceExtenderSetting
	{
		string TypeName { get; }
		bool Enabled { get; }
	}
}

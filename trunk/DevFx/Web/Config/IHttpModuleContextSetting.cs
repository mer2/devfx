/* Copyright(c) 2005-2011 R2@DevFx.NET, License(LGPL) */

namespace HTB.DevFx.Web.Config
{
	public interface IHttpModuleContextSetting
	{
		IHttpModuleSetting[] HttpApplications { get; }
	}
}

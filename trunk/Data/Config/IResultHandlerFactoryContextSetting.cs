/* Copyright(c) 2005-2011 R2@DevFx.NET, License(LGPL) */

namespace HTB.DevFx.Data.Config
{
	public interface IResultHandlerFactoryContextSetting
	{
		string FactoryTypeName { get; }
		bool ModuleEnabled { get; }
		IResultModuleSetting[] ResultModules { get; }
		IResultHandlerContextSetting ResultHandlerContext { get; }
	}
}

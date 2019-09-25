/* Copyright(c) 2005-2011 R2@DevFx.NET, License(LGPL) */

namespace DevFx.Data.Settings
{
	public interface IResultHandlerFactoryContextSetting
	{
		string FactoryTypeName { get; }
		bool ModuleEnabled { get; }
		string DefaultHandlerName { get; }
	}
}

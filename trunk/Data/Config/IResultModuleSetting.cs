/* Copyright(c) 2005-2011 R2@DevFx.NET, License(LGPL) */

using HTB.DevFx.Core.Config;

namespace HTB.DevFx.Data.Config
{
	public interface IResultModuleSetting : ITypeSetting
	{
		bool Enabled { get; }
	}
}

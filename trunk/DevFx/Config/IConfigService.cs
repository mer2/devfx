/* Copyright(c) 2005-2011 R2@DevFx.NET, License(LGPL) */

using System;

namespace HTB.DevFx.Config
{
	public interface IConfigService
	{
		IConfigSetting Settings { get; }
		IConfigSetting GetSetting<T>();
		IConfigSetting GetSetting(object target);
		IConfigSetting GetSetting(Type type);
		IConfigSetting GetSetting(string xpath);
	}
}

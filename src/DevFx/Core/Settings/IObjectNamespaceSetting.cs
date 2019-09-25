/* Copyright(c) 2005-2011 R2@DevFx.NET, License(LGPL) */

using DevFx.Configuration;

namespace DevFx.Core.Settings
{
	public interface IObjectNamespaceSetting
	{
		ITypeSetting[] TypeAliases { get; }
		IValueSetting[] ConstAliases { get; }
		IObjectSetting[] Objects { get; }
	}
}
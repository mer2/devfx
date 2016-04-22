/* Copyright(c) 2005-2011 R2@DevFx.NET, License(LGPL) */

namespace HTB.DevFx.Core.Config
{
	public interface IObjectNamespaceSetting
	{
		string Name { get; }
		ITypeSetting[] TypeAliases { get; }
		IValueSetting[] ConstAliases { get; }
		IObjectSetting[] ObjectSettings { get; }
		IObjectNamespaceSetting[] ObjectNamespaces { get; }
	}
}

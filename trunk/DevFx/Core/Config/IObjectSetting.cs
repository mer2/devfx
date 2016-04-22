/* Copyright(c) 2005-2011 R2@DevFx.NET, License(LGPL) */

namespace HTB.DevFx.Core.Config
{
	public interface IObjectSetting : Esb.Config.IObjectSetting
	{
		string Namespace { get; }
		string GroupName { get; }

		IConstructorSetting Constructor { get; }
		IValueSetting[] Properties { get; }
		IDependencySetting[] Dependencies { get; }

		ILifetimeSetting Lifetime { get; }
	}
}

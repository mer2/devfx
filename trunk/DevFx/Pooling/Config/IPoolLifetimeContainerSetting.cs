/* Copyright(c) 2005-2011 R2@DevFx.NET, License(LGPL) */

namespace HTB.DevFx.Pooling.Config
{
	public interface IPoolLifetimeContainerSetting
	{
		bool Enabled { get; }
		bool Debug { get; }
		int MaxPoolSize { get; }
	}
}

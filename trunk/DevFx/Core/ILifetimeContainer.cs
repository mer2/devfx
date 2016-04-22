/* Copyright(c) 2005-2011 R2@DevFx.NET, License(LGPL) */

using HTB.DevFx.Core.Config;

namespace HTB.DevFx.Core
{
	public interface ILifetimeContainer : ILifetimePolicy
	{
		void Init(IObjectSetting objectSetting, IObjectBuilder objectBuilder);
		void InitCompleted();

		IObjectSetting ObjectSetting { get; }
		IObjectBuilder ObjectBuilder { get; }
	}
}

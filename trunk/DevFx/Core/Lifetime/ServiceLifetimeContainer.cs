/* Copyright(c) 2005-2011 R2@DevFx.NET, License(LGPL) */

namespace HTB.DevFx.Core.Lifetime
{
	public class ServiceLifetimeContainer : SingletonLifetimeContainer
	{
		protected override void InitCompleted() {
			this.GetObjectInternal(null);
		}
	}
}
/* Copyright(c) 2005-2011 R2@DevFx.NET, License(LGPL) */

namespace DevFx.Core.Lifetime
{
	[TypeAliase("Service")]
	public class ServiceObjectContainer : SingletonObjectContainer
	{
		protected override void InitCompleted() {
			this.GetObjectInternal(null);
		}
	}
}
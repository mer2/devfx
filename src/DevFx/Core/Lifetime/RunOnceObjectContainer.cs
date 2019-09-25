/* Copyright(c) 2005-2011 R2@DevFx.NET, License(LGPL) */

namespace DevFx.Core.Lifetime
{
	[TypeAliase("RunOnce")]
	public class RunOnceObjectContainer : TransientObjectContainer
	{
		protected override void InitCompleted() {
			this.GetObjectInternal(null);
		}
	}
}

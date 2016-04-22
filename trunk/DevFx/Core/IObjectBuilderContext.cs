/* Copyright(c) 2005-2011 R2@DevFx.NET, License(LGPL) */

using HTB.DevFx.Core.Config;

namespace HTB.DevFx.Core
{
	public interface IObjectBuilderContext : IObjectContext
	{
		IObjectBuilder ObjectBuilder { get; }
		IObjectSetting ObjectSetting { get; }
		object ObjectInstance { get; set; }
	}
}

/* Copyright(c) 2005-2011 R2@DevFx.NET, License(LGPL) */

namespace DevFx.Core
{
	public interface IObjectBuilderContext : IObjectContext
	{
		IObjectService ObjectService { get; }
		IObjectBuilder ObjectBuilder { get; }
		IObjectDescription ObjectDescription { get; }
		object ObjectInstance { get; set; }
	}
}
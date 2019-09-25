/* Copyright(c) 2005-2011 R2@DevFx.NET, License(LGPL) */

using System.Collections;

namespace DevFx.Core
{
	internal class ObjectBuilderContext : ObjectContextBase, IObjectBuilderContext
	{
		public ObjectBuilderContext(IObjectService objectService, IObjectBuilder objectBuilder, IObjectDescription objectDescription, IDictionary items = null) : base(items) {
			this.ObjectService = objectService;
			this.ObjectBuilder = objectBuilder;
			this.ObjectDescription = objectDescription;
		}

		public IObjectService ObjectService { get; set; }
		public IObjectBuilder ObjectBuilder { get; set; }
		public IObjectDescription ObjectDescription { get; set; }
		public object ObjectInstance { get; set; }
	}
}
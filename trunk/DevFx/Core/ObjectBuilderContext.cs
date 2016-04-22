/* Copyright(c) 2005-2011 R2@DevFx.NET, License(LGPL) */

using System.Collections;
using HTB.DevFx.Core.Config;

namespace HTB.DevFx.Core
{
	internal class ObjectBuilderContext : ObjectContextBase, IObjectBuilderContext
	{
		public ObjectBuilderContext(IObjectBuilder objectBuilder, IObjectSetting objectSetting) : this(objectBuilder, objectSetting, null) {
		}

		public ObjectBuilderContext(IObjectBuilder objectBuilder, IObjectSetting objectSetting, IDictionary items) : base(items) {
			this.ObjectBuilder = objectBuilder;
			this.ObjectSetting = objectSetting;
		}

		public IObjectBuilder ObjectBuilder {
			get { return this.GetItem<IObjectBuilder>(); }
			private set { this.SetItem(value); }
		}

		public IObjectSetting ObjectSetting {
			get { return this.GetItem<IObjectSetting>(); }
			private set { this.SetItem(value); }
		}

		public object ObjectInstance {
			get { return this.GetItem(typeof(IObjectBuilderContext)); }
			set { this.SetItem(typeof(IObjectBuilderContext), value); }
		}
	}
}

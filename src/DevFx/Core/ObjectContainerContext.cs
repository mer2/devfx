using System.Collections;

namespace DevFx.Core
{
    internal class ObjectContainerContext : ObjectContextBase, IObjectContainerContext
	{
		public ObjectContainerContext(object objectKey, IObjectContainer container = null, IDictionary items = null) : base(items) {
			this.ObjectKey = objectKey;
			this.Container = container;
		}

		public object ObjectKey { get; }
		public IObjectContainer Container { get; set; }
		public IObjectNamespace Namespace { get; set; }
		public IObjectService ObjectService { get; set; }
	}
}

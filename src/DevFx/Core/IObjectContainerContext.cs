namespace DevFx.Core
{
	public interface IObjectContainerContext : IObjectContext
	{
		object ObjectKey { get; }
		IObjectContainer Container { get; set; }
		IObjectNamespace Namespace { get; }
		IObjectService ObjectService { get; }
	}
}

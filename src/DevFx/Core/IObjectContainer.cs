namespace DevFx.Core
{
	public interface IObjectContainer : ILifetimePolicy
	{
		void Init(IObjectDescription objectDescription, IObjectBuilder objectBuilder);
		void InitCompleted();

		IObjectDescription ObjectDescription { get; }
		IObjectBuilder ObjectBuilder { get; }
	}
}
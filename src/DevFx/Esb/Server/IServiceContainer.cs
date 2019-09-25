namespace DevFx.Esb.Server
{
	public interface IServiceContainer
	{
		string Name { get; }
		string AliasName { get; }
		string Authorization { get; }
	}
}

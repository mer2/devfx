namespace DevFx.Data.Settings
{
    public interface IConnectionStringSetting
    {
		string Name { get; }
		string ConnectionString { get; }
		string ProviderName { get; }
	}
}
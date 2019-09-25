namespace DevFx.Esb.Serialize
{
	[Service]
	public interface ISerializerFactory
	{
		ISerializer GetSerializer(string contentType);
		ISerializer Default { get; }
	}
}

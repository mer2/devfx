namespace DevFx.Esb.Serialize
{
	public abstract class SerializerFactory
	{
		public static ISerializerFactory Current => ObjectService.GetObject<ISerializerFactory>();
		public static ISerializer DefaultSerializer => Current.Default;

		public static ISerializer GetSerializer(string contentType) {
			return Current.GetSerializer(contentType);
		}
	}
}

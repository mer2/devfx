namespace DevFx.Web
{
	[Service]
	public interface IWebHostService
	{
		string Host {get; }
		int Port { get; }
	}
}

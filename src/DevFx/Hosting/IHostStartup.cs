namespace DevFx.Hosting
{
	public interface IHostStartup
	{
		void Init(IHostBuilderInternal builder);
	}
}

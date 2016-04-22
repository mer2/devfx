namespace HTB.DevFx.Data
{
	public interface IDataExecuteContext
	{
		T GetDataService<T>() where T : ISessionDataService;
		IDataSession GetDataSession();
		void Commit();
		void Rollback();
	}
}

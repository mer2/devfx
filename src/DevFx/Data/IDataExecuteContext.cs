namespace DevFx.Data
{
	public interface IDataExecuteContext
	{
		T GetDataService<T>() where T : class, ISessionDataService;
		IDataSession GetDataSession();
		void Commit();
		void Rollback();
	}
}

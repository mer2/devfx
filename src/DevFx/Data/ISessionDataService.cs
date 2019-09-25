using System;

namespace DevFx.Data
{
	public interface ISessionDataService : IDisposable
	{
		void SetDataSession(IDataSession session, bool inTransation);
	}
}

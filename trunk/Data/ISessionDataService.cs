using System;

namespace HTB.DevFx.Data
{
	public interface ISessionDataService : IDisposable
	{
		void SetDataSession(IDataSession session, bool inTransation);
	}
}

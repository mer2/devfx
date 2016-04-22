/* Copyright(c) 2005-2011 R2@DevFx.NET, License(LGPL) */

using System.Web;

namespace HTB.DevFx.Remoting.Web
{
	internal class RemotingServiceModule : IHttpModule
	{
		public void Init(HttpApplication context) {
			RemotingHelper.RemotingServiceInitialize();
		}

		public void Dispose() {
		}
	}
}

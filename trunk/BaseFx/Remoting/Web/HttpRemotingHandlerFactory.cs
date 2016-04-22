/* Copyright(c) 2005-2011 R2@DevFx.NET, License(LGPL) */

namespace HTB.DevFx.Remoting.Web
{
	internal class HttpRemotingHandlerFactory : System.Runtime.Remoting.Channels.Http.HttpRemotingHandlerFactory
	{
		public HttpRemotingHandlerFactory() {
			RemotingHelper.RemotingServiceInitialize();
		}
	}
}

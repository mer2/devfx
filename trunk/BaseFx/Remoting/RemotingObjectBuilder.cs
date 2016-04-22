/* Copyright(c) 2005-2011 R2@DevFx.NET, License(LGPL) */

using System;
using HTB.DevFx.Esb.Config;

namespace HTB.DevFx.Remoting
{
	internal class RemotingObjectBuilder : IRemotingObjectBuilder
	{
		public object CreateObject(IObjectSetting setting, Type objectType, string uri, params object[] parameters) {
			return Activator.GetObject(objectType, uri);
		}
	}
}

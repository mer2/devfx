/* Copyright(c) 2005-2011 R2@DevFx.NET, License(LGPL) */

using System;
using System.Reflection;
using System.ServiceModel;
using System.ServiceModel.Channels;
using HTB.DevFx.Esb.Config;

namespace HTB.DevFx.Remoting
{
	internal class WcfObjectBuilder : IRemotingObjectBuilder
	{
		public object CreateObject(IObjectSetting setting, Type objectType, string uri, params object[] parameters) {
			var factoryType = typeof (ChannelFactory<>).MakeGenericType(objectType);
			var methodCreateChannel = factoryType.GetMethod("CreateChannel", BindingFlags.Public | BindingFlags.Static, null, new[] {typeof (Binding), typeof (EndpointAddress)}, null);
			return methodCreateChannel.Invoke(null, new object[] { CreateBinding(setting, uri), new EndpointAddress(uri) });
		}

		private static Binding CreateBinding(IObjectSetting setting, string url) {
			string bindingName = null;
			var bc = setting.ConfigSetting["binding"];
			if(bc != null) {
				bindingName = bc.Value.Value;
			}
			if(string.IsNullOrEmpty(bindingName)) {
				var uri = new Uri(url, UriKind.Absolute);
				bindingName = uri.Scheme;
			}
			if(string.IsNullOrEmpty(bindingName)) {
				throw new RemotingException("Binding not found");
			}
			var binding = ObjectService.GetObject<Binding>("Wcf." + bindingName);
			if(binding == null) {
				throw new RemotingException("Binding not found");
			}
			return binding;
		}
	}
}

/* Copyright(c) 2005-2011 R2@DevFx.NET, License(LGPL) */

using System.ServiceModel;
using System.Web;
using HTB.DevFx.Remoting.Config;
using HTB.DevFx.Utils;

namespace HTB.DevFx.Remoting
{
	internal class WcfServiceInternal : RemotingServiceInternal
	{
		protected override void PublishService(RemotingServiceItemSetting setting) {
			var typeName = setting.TypeName;
			if(string.Compare(typeName, ":wcf", true) != 0) {
				base.PublishService(setting);
				return;
			}
			if(HttpContext.Current != null) {//ignore in iis
				return;
			}
			var serviceTypeName = setting.ServiceTypeName;
			if(string.IsNullOrEmpty(serviceTypeName)) {
				throw new RemotingException("serviceType is required in config file");
			}
			serviceTypeName = ObjectService.GetTypeName(setting.ServiceTypeName);
			var serviceType = TypeHelper.CreateType(serviceTypeName, true);
			var serviceHost = new ServiceHost(serviceType);
			serviceHost.Open();
		}
	}
}

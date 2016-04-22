/* Copyright(c) 2005-2011 R2@DevFx.NET, License(LGPL) */

using System;
using System.Runtime.Remoting;
using HTB.DevFx.Core;
using HTB.DevFx.Esb;
using HTB.DevFx.Remoting.Config;
using HTB.DevFx.Utils;

namespace HTB.DevFx.Remoting
{
	internal class RemotingServiceInternal : ObjectBase<RemotingServiceSetting>
	{
		protected override void OnInit() {
			base.OnInit();
			var configFile = this.Setting.ConfigFile;
			if(string.IsNullOrEmpty(configFile)) {
				configFile = AppDomain.CurrentDomain.SetupInformation.ConfigurationFile;
			}
			RemotingHelper.RegisterRemotingObject(configFile);
			foreach(var setting in this.Setting.Services) {
				this.PublishService(setting);
			}
		}

		protected virtual void PublishService(RemotingServiceItemSetting setting) {
			var typeName = setting.TypeName;
			if(string.IsNullOrEmpty(typeName)) {
				throw new RemotingException("type is missing in remoting config file");
			}
			typeName = ServiceLocator.GetTypeName(typeName);
			var serviceType = TypeHelper.CreateType(typeName, true);
			var serviceName = setting.ServiceTypeName;
			object serviceInstance = null;
			if(!string.IsNullOrEmpty(serviceName)) {
				serviceInstance = ServiceLocator.GetService(serviceName) ?? TypeHelper.CreateObject(serviceName, serviceType, false);
			}
			if(serviceInstance == null) {
				serviceInstance = TypeHelper.CreateObject(serviceType, null, false);
			}
			if(serviceInstance == null) {
				throw new RemotingException("remoting service instance can't be created");
			}
			var serviceProxy = serviceInstance as MarshalByRefObject ?? CreateServiceProxy(serviceType, serviceInstance);
			if(serviceProxy != null) {
				RemotingServices.Marshal(serviceProxy, setting.Name, serviceType);
			}
		}

		private static MarshalByRefObject CreateServiceProxy(Type serviceType, object serviceInstance) {
			var dynamicType = TypeHelper.CreateDynamic(serviceType, typeof(RemotingServiceBase<>).MakeGenericType(serviceType));
			return (MarshalByRefObject)Activator.CreateInstance(dynamicType, new[] { serviceInstance });
		}
	}
}

/* Copyright(c) 2005-2011 R2@DevFx.NET, License(LGPL) */

using System;
using System.Collections.Generic;
using HTB.DevFx.Core;
using HTB.DevFx.Esb;
using HTB.DevFx.Esb.Config;
using HTB.DevFx.Remoting.Config;
using HTB.DevFx.Utils;

namespace HTB.DevFx.Remoting
{
	internal class RemotingObjectBuilderFactory : ObjectBase<RemotingObjectBuilderFactorySetting>, IObjectBuilder
	{
		protected override void OnInit() {
			base.OnInit();
			var setting = this.Setting.HandlerContext;
			var defaultHandler = setting.DefaultHandler;
			if(!string.IsNullOrEmpty(defaultHandler)) {
				this.defaultBuilder = ServiceLocator.GetService<IRemotingObjectBuilder>(defaultHandler);
			}
			this.builders = new Dictionary<string, IRemotingObjectBuilder>();
			var handlers = setting.Handlers;
			if(handlers != null && handlers.Length > 0) {
				foreach(var handler in handlers) {
					var builder = ServiceLocator.GetService<IRemotingObjectBuilder>(handler.TypeName);
					if(builder != null) {
						this.builders.Add(handler.Name.ToLower(), builder);
					}
				}
			}
		}

		private IRemotingObjectBuilder defaultBuilder;
		private Dictionary<string, IRemotingObjectBuilder> builders;

		#region Implementation of IObjectBuilder

		public object CreateObject(IObjectSetting setting, params object[] parameters) {
			if(setting == null) {
				return null;
			}
			IRemotingObjectBuilder builder = null;
			var typeName = ServiceLocator.GetTypeName(setting.TypeName);
			var objectType = TypeHelper.CreateType(typeName, false);
			if(objectType == null) {
				return null;
			}
			var uri = setting.MapTo;
			var remotingSetting = SettingRequiredExtender.ToCustomSetting<RemotingObjectBuilderSetting>(setting);
			if(remotingSetting != null) {
				if(!string.IsNullOrEmpty(remotingSetting.BuilderType)) {
					builder = ServiceLocator.GetService<IRemotingObjectBuilder>(remotingSetting.BuilderType);
				}
				if(!string.IsNullOrEmpty(remotingSetting.Uri)) {
					uri = remotingSetting.Uri;
				}
			}
			object instance = null;
			if(builder == null) {
				if(!string.IsNullOrEmpty(uri)) {
					var index = uri.LastIndexOf('.');//按后缀查找对象创建器
					if(index > 0) {
						var handlerName = uri.Substring(index).ToLower();
						this.builders.TryGetValue(handlerName, out builder);
					}
					if (builder == null) {//按Scheme查找对象创建器
						index = uri.IndexOf("://", StringComparison.Ordinal);
						if (index > 0) {
							var handlerName = uri.Substring(0, index + 3).ToLower();
							this.builders.TryGetValue(handlerName, out builder);
						}
					}
				}
			}
			if(builder == null) {
				builder = this.defaultBuilder;
			}
			if(builder != null) {
				instance = builder.CreateObject(setting, objectType, uri, parameters);
			}
			return instance;
		}

		#endregion
	}
}

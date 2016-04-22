/* Copyright(c) 2005-2011 R2@DevFx.NET, License(LGPL) */

using System;
using System.Linq;
using System.Collections.Generic;
using System.Web;
using HTB.DevFx.Core;
using HTB.DevFx.Web.Config;

namespace HTB.DevFx.Web
{
	internal sealed class HttpModuleWrapper : IHttpModule
	{
		private IHttpModule currentModule;
		public void Init(HttpApplication context) {
			this.currentModule = ObjectService.GetObject<HttpModuleWrapperInternal>();
			if (this.currentModule != null) {
				this.currentModule.Init(context);
			}
		}

		public void Dispose() {
			if (this.currentModule != null) {
				this.currentModule.Dispose();
				this.currentModule = null;
			}
		}

		internal sealed class HttpModuleWrapperInternal : IHttpModule, IInitializable<IHttpModuleContextSetting>
		{
			public void Init(HttpApplication context) {
				if(this.httpModules != null && this.httpModules.Length > 0) {
					Array.ForEach(this.httpModules, x => x.Init(context));
				}
			}

			public void Dispose() {
				if (this.httpModules != null && this.httpModules.Length > 0) {
					Array.ForEach(this.httpModules, x => x.Dispose());
				}
			}

			private IHttpModule[] httpModules;
			public void Init(IHttpModuleContextSetting setting) {
				var apps = new List<IHttpModule>();
				if(setting != null && setting.HttpApplications != null && setting.HttpApplications.Length > 0) {
					Array.ForEach(setting.HttpApplications, x => {
						var typeName = x.TypeName;
						if(!string.IsNullOrEmpty(typeName)) {
							var app = ObjectService.Current.GetOrCreateObject<IHttpModule>(x.TypeName);
							if(app != null) {
								apps.Add(app);
							}
						}
					});
				}
				this.httpModules = apps.Where(x => !(x is HttpModuleWrapper || x is HttpModuleWrapperInternal)).ToArray();
			}
		}
	}
}

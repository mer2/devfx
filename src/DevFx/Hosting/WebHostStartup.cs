using DevFx.Core;
using DevFx.Esb.Server;
using DevFx.Web;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace DevFx.Hosting
{
	[Object, HostStartup]
	internal class WebHostStartup : IHostStartup, IStartupFilter, IWebHostService
	{
		[Autowired]
		protected IObjectService ObjectService { get; set; }
		protected Type[] MiddlewareTypes { get; set; }
		public string Host { get; private set; }
		public int Port { get; private set; }

		public void Init(IHostBuilderInternal builder) {
			if (!(builder.GetBuilderWrapper() is IWebHostBuilder hostBuilder)) {
				return;
			}

			var serverUrls = hostBuilder.GetSetting(WebHostDefaults.ServerUrlsKey);
			if(!string.IsNullOrEmpty(serverUrls)) {
				var urls = serverUrls.Split(';');
				Uri uri = null;
				foreach(var url in urls) {
					if(url.Contains("*")) {// http://*:5000
						var fakeurl = url.Replace('*', '_');
						if(Uri.TryCreate(fakeurl, UriKind.Absolute, out var uri1)) {
							uri = uri1;
							break;
						}
					} else {
						if (Uri.TryCreate(url, UriKind.Absolute, out var uri1)) {
							if(!uri1.IsLoopback) {
								uri = uri1;
							}
						}
					}
				}
				if(uri != null) {
					this.Port = uri.Port;
					if(uri.Host != "_") {
						this.Host = uri.Host;
					}
				}
			}

			var coreAttributes = this.ObjectService.AsObjectServiceInternal().CoreAttributes;
			if (coreAttributes.TryGetValue(typeof(MiddlewareAttribute), out var list)) {
				this.MiddlewareTypes = list.OrderByDescending(x => x.Priority).Select(x => x.OwnerType).ToArray();
			}

			builder.ConfigureServices(services => {
				//把自己加进去
				services.AddSingleton<IStartupFilter>(this);

				var middlewares = this.MiddlewareTypes;
				if (middlewares != null && middlewares.Length > 0) {
					foreach (var middleware in middlewares) {
						services.AddSingleton(middleware);
					}
				}
			});
		}

		Action<IApplicationBuilder> IStartupFilter.Configure(Action<IApplicationBuilder> next) {
			return app => {
				var middlewares = this.MiddlewareTypes;
				if (middlewares != null && middlewares.Length > 0) {
					foreach (var middleware in middlewares) {
						app.UseMiddleware(middleware);
					}
				}
				next(app);
				this.RegisterMiddleware(app);
			};
		}

		private void RegisterMiddleware(IApplicationBuilder app) {//注册Esb服务端处理程序
			var factory = this.ObjectService.GetObject<IServiceFactory>();
			app.UseWhen(context => factory.IsHandleable(context), appBuilder => {
				appBuilder.Use((ctx, next) => {
					factory.ProcessRequest(ctx);
					return Task.CompletedTask;
				});
			});
		}
	}
}

using DevFx.Hosting;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Net.Http;

namespace DevFx.Utils
{
	[Object, HostStartup]
	public sealed class HttpClientHelper : IHostStartup
	{
		private static HttpClientHelper currentInstance;
		public static IHttpClientFactory GetHttpClientFactory() {
			var instance = currentInstance ?? (currentInstance = ObjectService.GetObject<HttpClientHelper>());
			return instance.HttpClientFactory;
		}

		public static HttpClient CreateHttpClient(object name = null) {
			var factory = GetHttpClientFactory();
			return name == null ? factory.CreateClient() : factory.CreateClient(name.ToString());
		}

		private HttpClientHelper() {}
		private IHttpClientFactory httpFactory;
		private IHttpClientFactory HttpClientFactory {
			get {
				if (this.httpFactory == null) {
					lock (this) {
						if (this.httpFactory == null) {
							var serviceProvider = new ServiceCollection().AddHttpClient().BuildServiceProvider();
							this.httpFactory = serviceProvider.GetRequiredService<IHttpClientFactory>();
						}
					}
				}
				return this.httpFactory;
			}
			set {
				this.httpFactory = value;
			}
		}

		void IHostStartup.Init(IHostBuilderInternal builder) {
			builder.ConfigureServices(services => {
				var serviceProvider = services.AddHttpClient().BuildServiceProvider();
				this.httpFactory = serviceProvider.GetRequiredService<IHttpClientFactory>();
			});
		}
	}
}

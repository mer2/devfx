using System;
using System.Linq;
using System.Threading.Tasks;
using DevFx.Core;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace DevFx.Hosting
{
	public static class HostService
	{
		public static async Task Run(IHostBuilder builder = null) {
			var runConsole = false;
			if (builder == null) {
				builder = new HostBuilder();
				runConsole = true;
			}
			UseDevFx(builder);
			if (runConsole) {
				await builder.RunConsoleAsync();
			}
		}

		private static bool initialized;
	    internal static void Init(IObjectService objectService, IHostBuilderInternal builder) {
		    if (initialized) {//保证只初始化一次
				return;
		    }
			initialized = true;

			var coreAttributes = objectService.AsObjectServiceInternal().CoreAttributes;
			if (coreAttributes.TryGetValue(typeof(HostStartupAttribute), out var list)) {
				var startupTypes = list.OrderByDescending(x => x.Priority).Select(x => x.OwnerType).ToArray();
				foreach(var type in startupTypes) {
					var startup = objectService.GetOrCreateObject(type) as IHostStartup;
					startup?.Init(builder);
				}
			}
	    }

		/// <summary>
		/// 用于AspnetCore环境
		/// </summary>
		public static IWebHostBuilder UseDevFx(this IWebHostBuilder builder) {
			if (builder == null) {
				throw new ArgumentNullException(nameof(builder));
			}
			Init(ObjectService.Current, new WebHostBuilderWrapper(builder));
			return builder;
		}

		/// <summary>
		/// 用于Console环境
		/// </summary>
		public static IHostBuilder UseDevFx(this IHostBuilder builder) {
			if (builder == null) {
				throw new ArgumentNullException(nameof(builder));
			}
			Init(ObjectService.Current, new ConsoleHostBuilderWrapper(builder));
			return builder;
		}
	}

	internal class WebHostBuilderWrapper : IHostBuilderInternal
	{
		public WebHostBuilderWrapper(IWebHostBuilder builder) {
			this.Builder = builder;
		}
		internal IWebHostBuilder Builder { get; set; }

		public void ConfigureServices(Action<IServiceCollection> configureServices) {
			this.Builder.ConfigureServices(configureServices);
		}

		public object GetBuilderWrapper() {
			return this.Builder;
		}
	}

	internal class ConsoleHostBuilderWrapper : IHostBuilderInternal
	{
		public ConsoleHostBuilderWrapper(IHostBuilder builder) {
			this.Builder = builder;
		}
		internal IHostBuilder Builder { get; set; }

		public void ConfigureServices(Action<IServiceCollection> configureServices) {
			this.Builder.ConfigureServices(configureServices);
		}

		public object GetBuilderWrapper() {
			return this.Builder;
		}
	}
}

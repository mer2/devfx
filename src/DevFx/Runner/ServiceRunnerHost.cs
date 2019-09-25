/* Copyright(c) 2005-2011 R2@DevFx.NET, License(LGPL) */

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using DevFx.Core;
using DevFx.Hosting;
using DevFx.Logging;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace DevFx.Runner
{
	[Object, HostStartup]
	public class ServiceRunnerHost : IInitializable, IHostStartup
	{
		internal ServiceRunnerHost() { }
		[Autowired]
		protected IObjectService ObjectService { get; set; }
		[Autowired]
		protected ILogService LogService { get; set; }
		public IServiceRunner[] ServiceRunners { get; private set; }

		void IInitializable.Init() {
			var runners = new List<IServiceRunner>();
			var objectRunners = this.ObjectService.GetObjects<IServiceRunner>();
			if(objectRunners != null) {
				runners.AddRange(objectRunners);
			}

			var coreAttributes = this.ObjectService.AsObjectServiceInternal().CoreAttributes;
			//获取由TimerRunAttribute定义的Runner
			if (coreAttributes.TryGetValue(typeof(TimerRunAttribute), out var list)) {
				foreach(TimerRunAttribute attribute in list) {
					if (this.ObjectService.GetOrCreateObject(attribute.OwnerType) is IServiceHandler handler) {
						var runner = new TimerHost(attribute.Interval, handler);
						runners.Add(runner);
					}
				}
			}
			//获取由ScheduleRunAttribute定义的Runner
			if (coreAttributes.TryGetValue(typeof(ScheduleRunAttribute), out list)) {
				foreach (ScheduleRunAttribute attribute in list) {
					if (this.ObjectService.GetOrCreateObject(attribute.OwnerType) is IServiceHandler handler) {
						var ss = attribute.StartTime;
						DateTime startTime;
						if(string.IsNullOrEmpty(ss)) {
							startTime = DateTime.Now;
						} else {
							if(!DateTime.TryParse(ss, out startTime)) {
								startTime = DateTime.Now;
							}
						}
						var runner = new ScheduleHost(attribute.Interval, startTime, attribute.IntervalType, attribute.IntervalValue, handler, this.LogService);
						runners.Add(runner);
					}
				}
			}
			this.ServiceRunners = runners.ToArray();
		}

		public void Start() {
			this.StartServices(null);
		}

		public void StartServices(IServiceRunner[] serviceRunners) {
			if (serviceRunners == null) {
				serviceRunners = this.ServiceRunners;
			}
			if (serviceRunners != null && serviceRunners.Length > 0) {
				foreach (var runner in serviceRunners) {
					runner?.Start();
				}
			}
		}

		public void Stop() {
			this.StopServices(null);
		}

		public void StopServices(IServiceRunner[] serviceRunners) {
			if (serviceRunners == null) {
				serviceRunners = this.ServiceRunners;
			}
			if (serviceRunners != null && serviceRunners.Length > 0) {
				foreach (var runner in serviceRunners) {
					runner?.Stop();
				}
			}
		}

		public void Init(IHostBuilderInternal builder) {
			builder.ConfigureServices(services => {
				services.AddHostedService<ServiceRunning>();
			});
		}
	}

	internal class ServiceRunning : IHostedService
	{
		public Task StartAsync(CancellationToken cancellationToken) {
			if(StarterAttribute.IsEnabled<EnableRunnerAttribute>()) {
				var host = ObjectService.GetObject<ServiceRunnerHost>();
				host.Start();
			}
			return Task.CompletedTask;
		}

		public Task StopAsync(CancellationToken cancellationToken) {
			if (StarterAttribute.IsEnabled<EnableRunnerAttribute>()) {
				var host = ObjectService.GetObject<ServiceRunnerHost>();
				host.Stop();
			}
			return Task.CompletedTask;
		}
	}
}

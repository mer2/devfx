/* Copyright(c) 2005-2011 R2@DevFx.NET, License(LGPL) */

using System.Linq;
using HTB.DevFx.Core;
using HTB.DevFx.Services.Config;

namespace HTB.DevFx.Services
{
	public sealed class ServiceRunnerHost : ServiceBase<IServiceRunnerHostSetting>
	{
		internal ServiceRunnerHost() { }

		protected override void OnInit() {
			this.ServiceRunners = this.Setting.ServiceRunners.Where(x => x.Enabled)
				.Select(x => {
					var runner = this.ObjectService.GetOrCreateObject<IServiceRunner>(x.ServiceType);
					if(runner is IServiceRunnerInternal) {
						((IServiceRunnerInternal)runner).Init(x);
					}
					var title = x.ServiceName;
					if (!string.IsNullOrEmpty(x.Title)) {
						title = x.Title + "[" + title + "]";
					}
					return new RunnerWrap(title, runner);
				}).ToArray();
		}

		public RunnerWrap[] ServiceRunners { get; private set; }

		public void Start() {
			this.StartServices(null);
		}

		public void StartServices(RunnerWrap[] serviceRunners) {
			if(serviceRunners == null) {
				serviceRunners = this.ServiceRunners;
			}
			if(serviceRunners != null && serviceRunners.Length > 0) {
				foreach(var runner in serviceRunners) {
					if (runner.Runner != null) {
						runner.Runner.Start();
					}
				}
			}
		}

		public void Stop() {
			this.StopServices(null);
		}

		public void StopServices(RunnerWrap[] serviceRunners) {
			if (serviceRunners == null) {
				serviceRunners = this.ServiceRunners;
			}
			if (serviceRunners != null && serviceRunners.Length > 0) {
				foreach (var runner in serviceRunners) {
					if (runner.Runner != null) {
						runner.Runner.Stop();
					}
				}
			}
		}

		public static ServiceRunnerHost Current {
			get { return DevFx.ObjectService.GetObject<ServiceRunnerHost>(); }
		}
	}
}

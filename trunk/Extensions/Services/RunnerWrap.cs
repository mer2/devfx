/* Copyright(c) 2005-2011 R2@DevFx.NET, License(LGPL) */

namespace HTB.DevFx.Services
{
	public class RunnerWrap
	{
		public RunnerWrap(string serviceName, IServiceRunner runner) {
			this.ServiceName = serviceName;
			this.Runner = runner;
		}

		public string ServiceName { get; internal set; }
		public IServiceRunner Runner { get; internal set; }

		public override string ToString() {
			return this.ServiceName;
		}
	}
}
/* Copyright(c) 2005-2011 R2@DevFx.NET, License(LGPL) */

namespace HTB.DevFx.Services
{
	internal class TimerHost : TimerRunner
	{
		private readonly IServiceHandler serviceHandler;

		public TimerHost(IServiceHandler serviceHandler) {
			this.serviceHandler = serviceHandler;
		}

		public TimerHost(double interval, IServiceHandler serviceHandler) : base(interval) {
			this.serviceHandler = serviceHandler;
		}

		#region Overrides of TimerBase

		protected override bool Enabled {
			get { return base.Enabled; }
			set {
				base.Enabled = value;
				if (this.serviceHandler != null) {
					this.serviceHandler.Enabled = value;
				}
			}
		}

		/// <summary>
		/// 时间到了后引发的操作
		/// </summary>
		protected override void OnTimer() {
			if(this.serviceHandler != null) {
				this.serviceHandler.OnHandle(null);
			}
		}

		#endregion
	}
}

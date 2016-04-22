/* Copyright(c) 2005-2011 R2@DevFx.NET, License(LGPL) */

using System;
using HTB.DevFx.Log;

namespace HTB.DevFx.Services
{
	internal class ScheduleHost : ScheduleRunner
	{
		public ScheduleHost(double interval, DateTime startTime, DateIntervalType intervalType, int intervalValue, ILogService logService, IServiceHandler serviceHandler) : base(interval, startTime, intervalType, intervalValue, logService) {
			this.serviceHandler = serviceHandler;
		}
		private readonly IServiceHandler serviceHandler;

		#region Overrides of TimerBase

		protected override bool Enabled {
			get { return base.Enabled; }
			set {
				base.Enabled = value;
				if(this.serviceHandler != null) {
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

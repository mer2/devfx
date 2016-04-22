/* Copyright(c) 2005-2011 R2@DevFx.NET, License(LGPL) */

using System.Timers;
using HTB.DevFx.Utils;

namespace HTB.DevFx.Services
{
	/// <summary>
	/// 按一定时间间隔运行服务
	/// </summary>
	/// <remarks>
	/// 此服务不会重入，即只有一次服务完成后才进入下一次的计时
	/// </remarks>
	public abstract class TimerRunner : TimerBase, IServiceRunner
	{
		protected TimerRunner() { }

		protected TimerRunner(double interval) : base(interval) { }

		protected virtual bool Enabled { get; set; }

		private Timer scheduleTimer;
		protected override Timer ScheduleTimer {
			get { return this.scheduleTimer ?? (this.scheduleTimer = base.ScheduleTimer); }
		}

		protected virtual void StartInternal() {
			this.Enabled = true;
			this.StartTimer();
		}

		protected virtual void StopInternal() {
			this.Enabled = false;
			if (this.scheduleTimer != null) {
				this.ScheduleTimer.Stop();
			}
		}

		protected override void OnTimer(object sender, ElapsedEventArgs e) {
			if (this.Enabled) {
				base.OnTimer(sender, e);
			}
		}

		#region Implementation of IServiceRunner

		void IServiceRunner.Start() {
			this.StartInternal();
		}

		void IServiceRunner.Stop() {
			this.StopInternal();
		}

		#endregion
	}
}
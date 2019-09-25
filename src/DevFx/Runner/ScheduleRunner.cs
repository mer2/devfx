/* Copyright(c) 2005-2011 R2@DevFx.NET, License(LGPL) */

using System;
using DevFx.Logging;
using DevFx.Utils;

namespace DevFx.Runner
{
	public enum DateIntervalType
	{
		Year,
		Month,
		Week,
		Day,
		Hour,
		Minute
	}

	/// <summary>
	/// 指定起始时间，按指定间隔运行服务
	/// </summary>
	/// <remarks>
	/// 此服务会重入，只要指定时间一到即调用<see cref="TimerBase.OnTimer()"/>
	/// 重载<see cref="Overlap"/>指定是否重入，默认为可重入的
	/// 所以继承者需要考虑重入问题
	/// 配置格式
	///		interval="间隔毫秒数（默认为1000毫秒）（此属性作为检查是否到时间点用途）"
	///		startTime="服务运行开始时间"
	///		intervalType="时间间隔类型：year,month,week,day,hour,minute"
	///		intervalValue="间隔值（正整数）"
	/// 只需要重载<see cref="TimerRunner.OnTimer()"/>即可
	/// </remarks>
	public abstract class ScheduleRunner : TimerRunner
	{
		/// <summary>
		/// 构造函数
		/// </summary>
		/// <param name="interval">间隔毫秒数（默认为1000毫秒）（此属性作为检查是否到时间点用途）</param>
		/// <param name="startTime">服务运行开始时间</param>
		/// <param name="intervalType">时间间隔类型：year,month,week,day,hour,minute</param>
		/// <param name="intervalValue">间隔值（正整数）</param>
		protected ScheduleRunner(double interval, DateTime startTime, DateIntervalType intervalType, int intervalValue) : base(interval) {
			this.StartTime = startTime;
			this.IntervalType = intervalType;
			this.IntervalValue = intervalValue;
		}

		protected override void StartInternal() {
			this.NextTime = this.StartTime;
			this.EvalNextTime();
			this.LogService?.Info($"完成初始化，下次执行时间是：{this.NextTime}", this);
			base.StartInternal();
		}

		[Autowired(Required = true)]
		protected ILogService LogService { get; set; }
		protected virtual DateTime StartTime { get; set; }
		protected virtual DateIntervalType IntervalType { get; set; }
		protected virtual int IntervalValue { get; set; }

		protected virtual DateTime NextTime { get; set; }
		protected virtual bool Overlap { get { return true; } }
		protected virtual bool Running { get; set; }

		protected override bool OnTimer(DateTime signalTime) {
			if(this.NextTime < signalTime) {
				var lastTime = this.NextTime;
				this.NextTime = this.EvalNextTime(this.NextTime, this.IntervalValue);

				if (this.NextTime >= signalTime) {
					if (!this.Running || this.Overlap) {
						this.Running = true;
						this.LogService?.Info($"[{lastTime.Ticks}]正在执行", this);
						this.OnTimer();
						this.LogService?.Info($"[{lastTime.Ticks}]完成执行，下次执行时间是：{this.NextTime}", this);
						this.Running = false;
					}
				}
			}
			return true;
		}

		private void EvalNextTime() {
			var now = DateTime.Now;
			var total = now - this.NextTime;
			if(total < TimeSpan.Zero) {
				return;
			}
			var interval = -1D;
			switch(this.IntervalType) {
				case DateIntervalType.Year:
					interval = total.TotalDays/366;
					break;
				case DateIntervalType.Month:
					interval = total.TotalDays/31;
					break;
				case DateIntervalType.Week:
					interval = total.TotalDays/7;
					break;
				case DateIntervalType.Day:
					interval = total.TotalDays;
					break;
				case DateIntervalType.Hour:
					interval = total.TotalHours;
					break;
				case DateIntervalType.Minute:
					interval = total.TotalMinutes;
					break;
			}
			var nextTime = this.EvalNextTime(this.NextTime, Convert.ToInt32(interval/this.IntervalValue) * this.IntervalValue);
			while(nextTime < now) {
				nextTime = this.EvalNextTime(nextTime, this.IntervalValue);
			}
			
			this.NextTime = nextTime;
		}

		private DateTime EvalNextTime(DateTime currentTime, int intervalValue) {
			var nextTime = currentTime;
			var interval = intervalValue;

			switch(this.IntervalType) {
				case DateIntervalType.Year:
					nextTime = nextTime.AddYears(interval);
					break;
				case DateIntervalType.Month:
					nextTime = nextTime.AddMonths(interval);
					break;
				case DateIntervalType.Week:
					nextTime = nextTime.AddDays(interval * 7);
					break;
				case DateIntervalType.Day:
					nextTime = nextTime.AddDays(interval);
					break;
				case DateIntervalType.Hour:
					nextTime = nextTime.AddHours(interval);
					break;
				case DateIntervalType.Minute:
					nextTime = nextTime.AddMinutes(interval);
					break;
			}

			return nextTime;
		}
	}
}

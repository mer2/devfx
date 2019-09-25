using DevFx.Core;
using System;

namespace DevFx.Runner
{
	[Serializable, AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
	public sealed class ScheduleRunAttribute : CoreAttribute
	{
		public ScheduleRunAttribute(DateIntervalType intervalType, int intervalValue) {
			this.IntervalType = intervalType;
			this.IntervalValue = intervalValue;
		}

		public DateIntervalType IntervalType { get; set; }
		public int IntervalValue { get; set; }
		public double Interval { get; set; } = 1000;
		public string StartTime { get; set; }
	}
}

using DevFx.Core;
using System;

namespace DevFx.Runner
{
	[Serializable, AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
	public sealed class TimerRunAttribute : CoreAttribute
	{
		public TimerRunAttribute(int interval) {
			if(interval <= 0) {
				throw new ArgumentOutOfRangeException(nameof(interval));
			}
			this.Interval = interval;
		}

		public double Interval { get; set; }
	}
}

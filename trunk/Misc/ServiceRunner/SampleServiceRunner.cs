/* Copyright(c) 2005-2011 R2@DevFx.NET, License(LGPL) */

using System;
using HTB.DevFx.Log;
using HTB.DevFx.Services;

namespace HTB.DevFx.ServiceRunners
{
	internal sealed class SampleServiceRunner : ScheduleRunner
	{
		public SampleServiceRunner(double interval, DateTime startTime, DateIntervalType intervalType, int intervalValue, ILogService logService) : base(interval, startTime, intervalType, intervalValue, logService) {}

		/// <summary>
		/// 时间到了后引发的操作
		/// </summary>
		protected override void OnTimer() {
			this.LogService.WriteLog(this, LogLevel.INFO, "OnTimer");
		}
	}

	internal sealed class SampleServiceHandler : IServiceHandler
	{
		#region Implementation of IServiceHandler

		/// <summary>
		/// 供外部调用的方法，处理服务逻辑
		/// </summary>
		/// <param name="parameters">参数（如果有的话）</param>
		public void OnHandle(object parameters) {
			LogService.WriteLog(this, LogLevel.INFO, "OnHandle");
		}

		#endregion

		#region IServiceHandler Members

		bool IServiceHandler.Enabled { get; set; }

		#endregion
	}
}
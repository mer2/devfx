/* Copyright(c) 2005-2011 R2@DevFx.NET, License(LGPL) */

namespace HTB.DevFx.Services
{
	/// <summary>
	/// 服务运行基类（实现<see cref="IServiceRunner"/>接口，以期在ServiceRunner中运行）
	/// </summary>
	public abstract class ServiceRunnerBase : IServiceRunner
	{
		protected virtual void StartInternal() {
		}

		protected virtual void StopInternal() {
		}

		#region IServiceRunner Members

		void IServiceRunner.Start() {
			this.StartInternal();
		}

		void IServiceRunner.Stop() {
			this.StopInternal();
		}

		#endregion
	}
}
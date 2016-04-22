/* Copyright(c) 2005-2011 R2@DevFx.NET, License(LGPL) */

namespace HTB.DevFx.Services
{
	/// <summary>
	/// 服务运行器接口
	/// </summary>
	public interface IServiceRunner
	{
		/// <summary>
		/// 开始运行
		/// </summary>
		void Start();

		/// <summary>
		/// 停止运行
		/// </summary>
		void Stop();
	}
}

/* Copyright(c) 2005-2011 R2@DevFx.NET, License(LGPL) */

namespace DevFx.Runner
{
	/// <summary>
	/// 服务处理接口
	/// </summary>
	public interface IServiceHandler
	{
		/// <summary>
		/// 是否需要处理
		/// </summary>
		bool Enabled { get; set; }

		/// <summary>
		/// 供外部调用的方法，处理服务逻辑
		/// </summary>
		/// <param name="parameters">参数（如果有的话）</param>
		void OnHandle(object parameters);
	}
}

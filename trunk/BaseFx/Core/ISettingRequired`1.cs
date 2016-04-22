/* Copyright(c) 2005-2011 R2@DevFx.NET, License(LGPL) */

namespace HTB.DevFx.Core
{
	/// <summary>
	/// 对象需要配置信息的接口（泛型）
	/// </summary>
	/// <typeparam name="T">配置信息类型</typeparam>
	public interface ISettingRequired<in T>
	{
		/// <summary>
		/// 设置强类型的配置信息
		/// </summary>
		T Setting { set; }
	}
}

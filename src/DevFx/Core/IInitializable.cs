/* Copyright(c) 2005-2017 R2@DevFx.NET, License(LGPL) */

namespace DevFx.Core
{
	/// <summary>
	/// 可被初始化接口
	/// </summary>
	public interface IInitializable
	{
		/// <summary>
		/// 初始化
		/// </summary>
		void Init();
	}

	/// <summary>
	/// 可被初始化接口（泛型）
	/// </summary>
	/// <typeparam name="TSetting">初始化参数</typeparam>
	public interface IInitializable<in TSetting>
	{
		/// <summary>
		/// 初始化
		/// </summary>
		/// <param name="setting">初始化参数</param>
		void Init(TSetting setting);
	}
}

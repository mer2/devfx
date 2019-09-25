/* Copyright(c) 2005-2011 R2@DevFx.NET, License(LGPL) */

namespace DevFx.Data
{
	/// <summary>
	/// 数据存储服务接口
	/// </summary>
	[Service]
	public interface IDataService : IDataOperation
	{
		/// <summary>
		/// 开始一个会话
		/// </summary>
		/// <returns>数据会话接口</returns>
		IDataSession BeginSession();

		/// <summary>
		/// 开始一个会话（指定存储数据库）
		/// </summary>
		/// <param name="storageName">存储数据库名</param>
		/// <returns>数据会话接口</returns>
		IDataSession BeginSession(string storageName);

		/// <summary>
		/// 开始一个会话（指定存储数据库，并指示是否需要数据库事务支持）
		/// </summary>
		/// <param name="storageName">存储数据库名</param>
		/// <param name="beginTransation">是否需要数据库事务支持</param>
		/// <returns>数据会话接口</returns>
		IDataSession BeginSession(string storageName, bool beginTransation);
	}
}

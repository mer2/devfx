/* Copyright(c) 2005-2011 R2@DevFx.NET, License(LGPL) */

using System;

namespace DevFx.Data
{
	/// <summary>
	/// 数据存储会话接口
	/// </summary>
	public interface IDataSession : IDataOperation, IDisposable
	{
		/// <summary>
		/// 提交事务
		/// </summary>
		void CommitTransaction();

		/// <summary>
		/// 回滚事务
		/// </summary>
		void RollbackTransaction();
	}
}

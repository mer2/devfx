/* Copyright(c) 2005-2011 R2@DevFx.NET, License(LGPL) */

using System.Collections;

namespace DevFx.Data.Results
{
	/// <summary>
	/// 实体初始化接口
	/// </summary>
	public interface IEntityInitialize
	{
		/// <summary>
		/// 初始化实体
		/// </summary>
		/// <param name="values">键值对列表</param>
		void InitValues(IDictionary values);
	}
}

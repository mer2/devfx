/* Copyright(c) 2005-2011 R2@DevFx.NET, License(LGPL) */

using System.Collections;

namespace HTB.DevFx.Data.Entities
{
	/// <summary>
	/// 实体接口
	/// </summary>
	public interface IEntity
	{
		/// <summary>
		/// 获取实体已被变更的属性列表
		/// </summary>
		/// <returns>已变更的键值列表</returns>
		IDictionary GetChangedValues();
	}
}
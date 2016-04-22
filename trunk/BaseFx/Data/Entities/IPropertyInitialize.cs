/* Copyright(c) 2005-2011 R2@DevFx.NET, License(LGPL) */

namespace HTB.DevFx.Data.Entities
{
	/// <summary>
	/// 实体属性初始化接口
	/// </summary>
	public interface IPropertyInitialize
	{
		/// <summary>
		/// 设置属性值
		/// </summary>
		/// <param name="propertyName">属性名称</param>
		/// <param name="propertyValue">属性值</param>
		void SetValue(string propertyName, object propertyValue);
	}
}

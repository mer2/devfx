/* Copyright(c) 2005-2011 R2@DevFx.NET, License(LGPL) */

namespace DevFx.Reflection
{
	/// <summary>
	/// 属性描述接口
	/// </summary>
	public interface IPropertyDescriptor
	{
		/// <summary>
		/// 获取属性值
		/// </summary>
		/// <param name="propertyName">属性名称</param>
		/// <returns>属性值</returns>
		object GetValue(string propertyName);

		/// <summary>
		/// 设置属性值
		/// </summary>
		/// <param name="propertyName">属性名称</param>
		/// <param name="newValue">属性值</param>
		void SetValue(string propertyName, object newValue);
	}
}
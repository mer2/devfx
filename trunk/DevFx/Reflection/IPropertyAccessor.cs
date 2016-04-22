/* Copyright(c) 2005-2011 R2@DevFx.NET, License(LGPL) */

namespace HTB.DevFx.Reflection
{
	/// <summary>
	/// 属性操作者接口
	/// </summary>
	public interface IPropertyAccessor
	{
		/// <summary>
		/// 属性信息
		/// </summary>
		IFieldOrPropertyInfo Property { get; }

		/// <summary>
		/// 获取属性值
		/// </summary>
		/// <param name="instance">对象实例</param>
		/// <returns>属性值</returns>
		object GetValue(object instance);
		
		/// <summary>
		/// 设置属性值
		/// </summary>
		/// <param name="instance">对象实例</param>
		/// <param name="value">属性值</param>
		void SetValue(object instance, object value);
	}
}

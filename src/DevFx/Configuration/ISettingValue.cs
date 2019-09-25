/* Copyright(c) 2005-2011 R2@DevFx.NET, License(LGPL) */

using System;
using DevFx.Utils;

namespace DevFx.Configuration
{
	/// <summary>
	/// 配置值接口
	/// </summary>
	/// <remarks>
	///	形如下面的XML节表示一个配置节：
	///		<code>
	///			&lt;app my="myProperty"&gt;myValue&lt;/app&gt;
	///		</code>
	///	此时，<c>Name="app"</c>，Value的值为"myValue"，Property的值为"myProperty"
	/// </remarks>
	public interface ISettingValue : ICloneable, IConverting
	{
		/// <summary>
		/// 当前配置值是否只读
		/// </summary>
		bool ReadOnly { get; }

		/// <summary>
		/// 配置值名
		/// </summary>
		string Name { get; }

		/// <summary>
		/// 配置值
		/// </summary>
		string Value { get; }

		/// <summary>
		/// 多配置值
		/// </summary>
		object[] Values { get; }

		/// <summary>
		///  克隆配置值
		/// </summary>
		/// <param name="readonly">是否只读</param>
		/// <returns>ISettingValue</returns>
		ISettingValue Clone(bool @readonly);
	}
}
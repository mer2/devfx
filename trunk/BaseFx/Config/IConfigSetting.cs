/* Copyright(c) 2005-2011 R2@DevFx.NET, License(LGPL) */

using System;

namespace HTB.DevFx.Config
{
	/// <summary>
	/// 配置节接口
	/// </summary>
	/// <remarks>
	///	形如下面的XML节表示一个配置节：
	///		<code>
	///			&lt;app my="myProperty"&gt;myValue&lt;/app&gt;
	///		</code>
	///	此时，<c>Name="app"</c>，Value的值为"myValue"，Property的值为"myProperty"
	/// </remarks>
	public interface IConfigSetting : ICloneable
	{
		/// <summary>
		/// 当前配置节是否只读
		/// </summary>
		bool ReadOnly { get; }

		/// <summary>
		/// 此配置节的名（逻辑名称，可能会根据配置命令而变化）
		/// </summary>
		string Name { get; }

		/// <summary>
		/// 此配置节实际名称（原始名称，可能为操作命令，比如“Add”等）
		/// </summary>
		string SettingName { get; }

		/// <summary>
		/// 此配置节的名/值
		/// </summary>
		ISettingValue Value { get; }

		/// <summary>
		/// 包含此配置节的父配置节
		/// </summary>
		IConfigSetting Parent { get; }

		/// <summary>
		/// 此配置节包含的直接子配置节数目
		/// </summary>
		int Children { get; }

		/// <summary>
		/// 配置节属性
		/// </summary>
		ISettingProperty Property { get; }

		/// <summary>
		/// 配置节版本（<see cref="Merge"/>调用后将影响此值）
		/// </summary>
		int Version { get; }

		/// <summary>
		/// 获取子配置节
		/// </summary>
		/// <param name="childSettingName">子配置节名</param>
		/// <returns>配置节</returns>
		/// <remarks>
		/// 如果不存在，将返回<c>null</c>
		/// </remarks>
		IConfigSetting this[string childSettingName] { get; }

		/// <summary>
		/// 获取子配置节
		/// </summary>
		/// <param name="childSettingIndex">子配置节顺序</param>
		/// <returns>配置节</returns>
		/// <remarks>
		/// 如果不存在，将返回null
		/// </remarks>
		IConfigSetting this[int childSettingIndex] { get; }

		/// <summary>
		/// 获取配置节的名称（逻辑名称，可能会根据配置命令而变化）
		/// </summary>
		/// <returns></returns>
		string GetName();

		/// <summary>
		/// 获取所有子配置节
		/// </summary>
		/// <returns>配置节数组</returns>
		IConfigSetting[] GetChildSettings();

		/// <summary>
		/// 获取指定名称的子配置节
		/// </summary>
		/// <param name="childSettingName">子配置节名称</param>
		/// <returns>配置节数组</returns>
		IConfigSetting[] GetChildSettings(string childSettingName);

		/// <summary>
		/// 按XPath方式获取配置节
		/// </summary>
		/// <param name="xpath">XPath</param>
		/// <returns>配置节</returns>
		/// <remarks>
		/// XPath为类似XML的XPath，形如<c>framework/modules"</c><br />
		/// 如果有相同的配置节，则返回第一个配置节
		/// </remarks>
		IConfigSetting GetChildSetting(string xpath);

		/// <summary>
		/// 按多级方式获取配置节
		/// </summary>
		/// <param name="settingName">多级的配置节名</param>
		/// <returns>配置节</returns>
		/// <remarks>
		/// 多级的配置节名，形如有如下配置：
		///		<code>
		///			&lt;app1&gt;
		///				&lt;app2&gt;
		///					&lt;app3&gt;&lt;/app3&gt;
		///				&lt;/app2&gt;
		///			&lt;/app1&gt;
		///		</code>
		///	则按顺序传入，比如<c>GetChildSetting("app1", "app2", "app3")</c>，此时返回名为<c>app3</c>的配置节
		/// </remarks>
		IConfigSetting GetChildSetting(params string[] settingName);

		/// <summary>
		/// 获取根配置节
		/// </summary>
		/// <returns>配置节</returns>
		IConfigSetting GetRootSetting();

		/// <summary>
		/// 合并配置节
		/// </summary>
		/// <param name="setting">被合并的配置节</param>
		/// <returns>配置节</returns>
		IConfigSetting Merge(IConfigSetting setting);

		/// <summary>
		/// 克隆此配置节
		/// </summary>
		/// <param name="readonly">是否只读</param>
		/// <param name="deep">是否进行深层次的克隆</param>
		/// <returns>配置节</returns>
		IConfigSetting Clone(bool @readonly, bool deep);

		/// <summary>
		/// 获取配置命令集合（为后续配置合并提供支持）
		/// </summary>
		/// <returns>配置节集合</returns>
		IConfigSettingCollection GetOperatorSettings();

		/// <summary>
		/// 设置配置节的父节点（为后续配置合并提供支持）
		/// </summary>
		/// <param name="parent">父节点</param>
		/// <returns>配置后的本配置节</returns>
		IConfigSetting SetParent(IConfigSetting parent);
	}
}

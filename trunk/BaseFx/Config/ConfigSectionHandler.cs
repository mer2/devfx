/* Copyright(c) 2005-2011 R2@DevFx.NET, License(LGPL) */

using System.Xml;

namespace HTB.DevFx.Config
{
	/// <summary>
	/// 实现对配置在<c>web.config</c>中的Xml进行<see cref="IConfigSetting"/>实例化
	/// </summary>
	/// <remarks>
	///		<para>提供一种通用的处理配置在<c>web.config，app.config</c>的自定义配置节处理方式</para>
	///		<para>按下面的方式添加：</para>
	///		<code>
	///			&lt;configuration&gt;
	///				&lt;configSections&gt;
	///					&lt;section name="htb.devfx" type="HTB.DevFx.Config.ConfigSectionHandler, HTB.DevFx.BaseFx" /&gt;
	///				&lt;/configSections&gt;
	///				......
	///				
	///				&lt;htb.devfx&gt;
	///					&lt;startup&gt;
	///					......
	///					&lt;/startup&gt;
	///					......
	///				&lt;/htb.devfx&gt;
	///				
	///				......
	///			&lt;/configuration&gt;
	///		</code>
	/// </remarks>
	public class ConfigSectionHandler : ConfigSectionHandlerBase<IConfigSetting>, IConfigSettingElement
	{
		/// <summary>
		/// 创建配置节实例
		/// </summary>
		/// <param name="section"><see cref="XmlNode"/>实例</param>
		/// <returns>配置节实例</returns>
		protected override IConfigSetting CreateSetting(XmlNode section) {
			return ConfigHelper.CreateFromXmlNode(section);
		}

		/// <summary>
		/// 合并配置节
		/// </summary>
		/// <param name="currentSetting">当前配置节</param>
		/// <param name="parentSetting">父配置节</param>
		/// <returns>合并后的配置节</returns>
		protected override IConfigSetting MergeSetting(IConfigSetting currentSetting, IConfigSetting parentSetting) {
			this.ConfigSetting = parentSetting.Clone(false, true).Merge(currentSetting);
			return this.ConfigSetting;
		}

		/// <summary>
		/// 获取或设置此强类型对应的配置节
		/// </summary>
		protected virtual IConfigSetting ConfigSetting { get; set; }

		#region IConfigSettingElement 成员

		IConfigSetting IConfigSettingElement.ConfigSetting {
			get { return this.ConfigSetting; }
			set { this.ConfigSetting = value; }
		}

		#endregion
	}
}
/* Copyright(c) 2005-2011 R2@DevFx.NET, License(LGPL) */

using System.Xml;
using System.Configuration;

namespace HTB.DevFx.Config
{
	/// <summary>
	/// 配置节处理基类（泛型）
	/// </summary>
	/// <typeparam name="T">配置节实际类型</typeparam>
	public abstract class ConfigSectionHandlerBase<T> : IConfigurationSectionHandler where T : class
	{
		/// <summary>
		/// 创建配置节实例
		/// </summary>
		/// <param name="section"><see cref="XmlNode"/>实例</param>
		/// <returns>配置节实例</returns>
		protected abstract T CreateSetting(XmlNode section);

		/// <summary>
		/// 合并配置节
		/// </summary>
		/// <param name="currentSetting">当前配置节</param>
		/// <param name="parentSetting">父配置节</param>
		/// <returns>合并后的配置节</returns>
		protected abstract T MergeSetting(T currentSetting, T parentSetting);

		/// <summary>
		/// 从配置文件配置节中获取配置节实例
		/// </summary>
		/// <param name="sectionNames">配置节名列表（找到为止）</param>
		/// <returns>配置节实例</returns>
		public static T GetConfig(params string[] sectionNames) {
			return ConfigHelper.GetSectionFromConfiguration<T>(false, sectionNames);
		}

		#region IConfigurationSectionHandler Members

		object IConfigurationSectionHandler.Create(object parent, object configContext, XmlNode section) {
			var currentSetting = this.CreateSetting(section);
			var parentSetting = (T)parent;
			if (parentSetting != null) {
				currentSetting = this.MergeSetting(currentSetting, parentSetting);
			}
			return currentSetting;
		}

		#endregion IConfigurationSectionHandler Members
	}
}

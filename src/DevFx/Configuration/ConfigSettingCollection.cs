/* Copyright(c) 2005-2011 R2@DevFx.NET, License(LGPL) */

using System.Collections.Generic;
using DevFx.Utils;

namespace DevFx.Configuration
{
	/// <summary>
	/// 配置节集合
	/// </summary>
	public class ConfigSettingCollection : CollectionBase<ConfigSetting>, IConfigSettingCollection
	{
		/// <summary>
		/// 构造方法
		/// </summary>
		/// <param name="uniqueKey">键值是否唯一</param>
		public ConfigSettingCollection(bool uniqueKey) : base(uniqueKey) { }

		/// <summary>
		/// 添加配置节
		/// </summary>
		/// <param name="setting">配置节</param>
		/// <returns>配置节</returns>
		public virtual ConfigSetting Add(ConfigSetting setting) {
			this.Add(setting.Name, setting);
			return setting;
		}

		/// <summary>
		/// 批量添加配置节
		/// </summary>
		/// <param name="settings">配置节列表</param>
		public virtual void AddRange(IEnumerable<ConfigSetting> settings) {
			foreach(var setting in settings) {
				this.Add(setting);
			}
		}

		/// <summary>
		/// 添加/替换配置节（如果存在则替换）
		/// </summary>
		/// <param name="setting">配置节</param>
		public virtual ConfigSetting Set(ConfigSetting setting) {
			this.Set(setting.Name, setting);
			return setting;
		}

		/// <summary>
		/// 深度复制集合
		/// </summary>
		/// <param name="parent">父配置节</param>
		/// <returns>复制后的集合</returns>
		public virtual ConfigSettingCollection Clone(ConfigSetting parent) {
			var collection = new ConfigSettingCollection(this.UniqueKey);
			foreach(var setting in this.Values) {
				collection.Add(setting.Clone()).Parent = parent;
			}
			return collection;
		}

		#region IConfigSettingCollection Members

		IConfigSetting IConfigSettingCollection.Add(IConfigSetting setting) {
			return this.Add((ConfigSetting)setting);
		}

		IConfigSetting IConfigSettingCollection.Set(IConfigSetting setting) {
			return this.Set((ConfigSetting)setting);
		}

		IConfigSettingCollection IConfigSettingCollection.Clone(IConfigSetting parent) {
			return this.Clone((ConfigSetting)parent);
		}

		IConfigSetting[] IConfigSettingCollection.Values {
			get { return this.Values; }
		}

		#endregion
	}
}
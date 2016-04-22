/* Copyright(c) 2005-2011 R2@DevFx.NET, License(LGPL) */

namespace HTB.DevFx.Config
{
	/// <summary>
	/// 强类型配置基类
	/// </summary>
	public abstract partial class ConfigSettingElement : IConfigSettingElement
	{
		private IConfigSetting configSetting;
		private int configVersion;

		/// <summary>
		/// 获取或设置此强类型对应的配置节
		/// </summary>
		public virtual IConfigSetting ConfigSetting {
			get { return this.configSetting; }
			protected set {
				if(this.configSetting == value) {
					if(value != null && this.configVersion == value.Version) {
						return;
					}
				}
				this.configSetting = value;
				if (value != null) {
					this.configVersion = value.Version;
				}
				this.OnConfigSettingChanged();
			}
		}

		/// <summary>
		/// 配置节变化时将调用的方法
		/// </summary>
		protected virtual void OnConfigSettingChanged() {
		}

		#region IConfigSettingElement 成员

		IConfigSetting IConfigSettingElement.ConfigSetting {
			get { return this.ConfigSetting; }
			set { this.ConfigSetting = value; }
		}

		#endregion
	}
}

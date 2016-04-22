/* Copyright(c) 2005-2011 R2@DevFx.NET, License(LGPL) */

using HTB.DevFx.Utils;

namespace HTB.DevFx.Config
{
	/// <summary>
	/// 配置值集合
	/// </summary>
	public class SettingValueCollection : CollectionBase<SettingValue>
	{
		/// <summary>
		/// 构造方法
		/// </summary>
		public SettingValueCollection() : base(true) {}

		/// <summary>
		/// 添加配置值
		/// </summary>
		/// <param name="value">配置值</param>
		/// <returns>配置值</returns>
		public virtual SettingValue Add(SettingValue value) {
			this.Add(value.Name, value);
			return value;
		}

		/// <summary>
		/// 添加/替换配置值（如果存在则替换）
		/// </summary>
		/// <param name="value">配置值</param>
		public virtual void Set(SettingValue value) {
			this.Set(value.Name, value);
		}
	}
}
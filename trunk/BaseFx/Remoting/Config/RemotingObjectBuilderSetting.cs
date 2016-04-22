/* Copyright(c) 2005-2011 R2@DevFx.NET, License(LGPL) */

using HTB.DevFx.Config;

namespace HTB.DevFx.Remoting.Config
{
	/// <summary>
	/// 远程服务创建器配置
	/// </summary>
	public class RemotingObjectBuilderSetting : ConfigSettingElement
	{
		/// <summary>
		/// 配置节变化时将调用的方法
		/// </summary>
		protected override void OnConfigSettingChanged() {
			this.Uri = this.GetSetting("uri");
			this.BuilderType = this.GetSetting("builderType");
		}

		/// <summary>
		/// 远程服务地址
		/// </summary>
		public virtual string Uri { get; private set; }

		/// <summary>
		/// 远程服务创建器类型
		/// </summary>
		public virtual string BuilderType { get; private set; }
	}
}

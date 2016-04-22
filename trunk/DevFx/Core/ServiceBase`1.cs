/* Copyright(c) 2005-2011 R2@DevFx.NET, License(LGPL) */

namespace HTB.DevFx.Core
{
	public abstract class ServiceBase<TSetting> : ServiceBase, ISettingRequired, ISettingRequired<TSetting>
	{
		protected virtual TSetting Setting { get; set; }

		#region ISettingRequired<T> Members

		TSetting ISettingRequired<TSetting>.Setting {
			set { this.Setting = value; }
		}

		#endregion

		#region ISettingRequired Members

		object ISettingRequired.Setting {
			set { this.Setting = (TSetting)value; }
		}

		#endregion
	}
}

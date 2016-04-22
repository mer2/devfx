/* Copyright(c) 2005-2011 R2@DevFx.NET, License(LGPL) */

using HTB.DevFx.Utils;

namespace HTB.DevFx.Core
{
	public abstract class Service<TSetting> : ServiceBase<TSetting> where TSetting : class
	{
		/// <summary>
		/// 当未通过<see cref="ObjectService"/>获取实例时，实例是未被初始化的，此方法保证实例是已被初始化的。
		/// </summary>
		protected virtual void Initialize() {
			ThreadHelper.ThreadSafeExecute(this, () => !this.Initialized, () => {
				this.Initialized = true;
				this.Setting = this.ObjectService.GetObjectTypedSetting<TSetting>(this.GetType());
				this.OnInit();
			});
		}
	}
}

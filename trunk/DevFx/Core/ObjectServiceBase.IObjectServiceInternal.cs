/* Copyright(c) 2005-2011 R2@DevFx.NET, License(LGPL) */

using HTB.DevFx.Config;

namespace HTB.DevFx.Core
{
	partial class ObjectServiceBase : IObjectServiceInternal
	{
		#region IObjectServiceInternal Members

		void IInitializable<IConfigService>.Init(IConfigService configService) {
			this.currentConfigService = configService;
			((IInitializable)this).Init();
		}

		void IObjectServiceInternal.InitCompleted() {
			this.InitCompletedInternal();
		}

		#endregion
	}
}

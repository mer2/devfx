/* Copyright(c) 2005-2011 R2@DevFx.NET, License(LGPL) */

namespace HTB.DevFx.Core
{
	public abstract class ObjectServiceExtenderBase : IObjectExtender<IObjectService>
	{
		protected virtual void Init(IObjectService objectService) {
			this.ObjectService = objectService;
		}

		protected virtual IObjectService ObjectService { get; private set; }

		#region IObjectExtender<IObjectService> Members

		void IObjectExtender<IObjectService>.Init(IObjectService objectService) {
			this.Init(objectService);
		}

		#endregion
	}
}

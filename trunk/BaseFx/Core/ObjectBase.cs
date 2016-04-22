/* Copyright(c) 2005-2011 R2@DevFx.NET, License(LGPL) */

using System;
using HTB.DevFx.Utils;

namespace HTB.DevFx.Core
{
	internal class ObjectBase : MarshalByRefObject, IService, IInitializable
	{
		protected virtual bool Initialized { get; set; }

		protected virtual void OnInit() {
		}

		#region IInitializable Members

		void IInitializable.Init() {
			ThreadHelper.ThreadSafeExecute(this, () => !this.Initialized, () => {
				this.Initialized = true;
				this.OnInit();
			});
		}

		#endregion
	}
}

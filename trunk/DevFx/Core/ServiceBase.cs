/* Copyright(c) 2005-2011 R2@DevFx.NET, License(LGPL) */

using System;
using HTB.DevFx.Config;
using HTB.DevFx.Reflection;
using HTB.DevFx.Utils;

namespace HTB.DevFx.Core
{
	public abstract class ServiceBase : MarshalByRefObject, IService, IInitializable, IPropertyDescriptor
	{
		protected virtual bool Initialized { get; set; }

		protected virtual void OnInit() {
		}

		protected virtual IConfigService ConfigService {
			get { return DevFx.ObjectService.GetObject<IConfigService>(); }
		}

		protected virtual IObjectService ObjectService {
			get { return DevFx.ObjectService.GetObject<IObjectService>(); }
		}

		protected virtual object GetPropertyValue(string propertyName) {
			return ReflectionHelper.GetPropertyValue(this, propertyName);
		}

		protected virtual void SetPropertyValue(string propertyName, object newValue) {
			TypeHelper.SetPropertyValue(this, propertyName, newValue);
		}

		#region IInitializable Members

		void IInitializable.Init() {
			ThreadHelper.ThreadSafeExecute(this, () => !this.Initialized, () => {
				this.Initialized = true;
				this.OnInit();
			});
		}

		#endregion

		#region IPropertyDescriptor Members

		object IPropertyDescriptor.GetValue(string propertyName) {
			return this.GetPropertyValue(propertyName);
		}

		void IPropertyDescriptor.SetValue(string propertyName, object newValue) {
			this.SetPropertyValue(propertyName, newValue);
		}

		#endregion
	}
}

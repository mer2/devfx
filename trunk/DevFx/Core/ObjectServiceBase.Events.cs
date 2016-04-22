/* Copyright(c) 2005-2011 R2@DevFx.NET, License(LGPL) */

using System;
using System.Collections;

namespace HTB.DevFx.Core
{
	partial class ObjectServiceBase
	{
		public event Action<IObjectServiceContext> PreInit;
		public event Action<IObjectServiceContext> InitCompleted;
		public event Action<IObjectServiceContext> Disposing;
		public event Action<IObjectServiceContext, Exception> Error;

		protected void OnPreInit() {
			this.OnPreInit(null);
		}

		protected virtual void OnPreInit(IDictionary items) {
			if (this.PreInit != null) {
				this.PreInit(new ObjectServiceContext(this, items));
			}
		}

		protected void OnInitCompleted() {
			this.OnInitCompleted(null);
		}

		protected virtual void OnInitCompleted(IDictionary items) {
			if (this.InitCompleted != null) {
				this.InitCompleted(new ObjectServiceContext(this, items));
			}
		}

		protected void OnDisposing() {
			this.OnDisposing(null);
		}

		protected virtual void OnDisposing(IDictionary items) {
			if (this.Disposing != null) {
				this.Disposing(new ObjectServiceContext(this, items));
			}
		}

		protected void OnError(Exception e) {
			this.OnError(e, null);
		}

		protected virtual void OnError(Exception e, IDictionary items) {
			if (this.Error != null) {
				this.Error(new ObjectServiceContext(this, items), e);
			}
		}
	}
}

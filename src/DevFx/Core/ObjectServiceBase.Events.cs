/* Copyright(c) 2005-2011 R2@DevFx.NET, License(LGPL) */

using System;

namespace DevFx.Core
{
	partial class ObjectServiceBase
	{
		public event Action<IObjectServiceContext> PreInit;
		public event Action<IObjectServiceContext> InitCompleted;
		public event Action<IObjectServiceContext> SetupCompleted;
		public event Action<IObjectServiceContext> Disposing;
		public event Action<IObjectServiceContext, Exception> Error;
		public event Action<IObjectContainerContext> ObjectContainerGetting;
		public event Action<IObjectContainerContext> ObjectContainerGetted;
		public event Action<IObjectBuilderContext> ObjectCreating;
		public event Action<IObjectBuilderContext> ObjectCreated;

		protected virtual void OnPreInit(IObjectServiceContext ctx) {
			this.PreInit?.Invoke(ctx);
		}

		protected virtual void OnInitCompleted(IObjectServiceContext ctx) {
			this.InitCompleted?.Invoke(ctx);
		}

		protected virtual void OnSetupCompleted(IObjectServiceContext ctx) {
			this.SetupCompleted?.Invoke(ctx);
		}

		protected virtual void OnDisposing(IObjectServiceContext ctx) {
			this.Disposing?.Invoke(ctx);
		}

		protected virtual void OnError(IObjectServiceContext ctx, Exception e) {
			this.Error?.Invoke(ctx, e);
		}

		protected virtual void OnObjectContainerGetting(IObjectContainerContext ctx) {
			this.ObjectContainerGetting?.Invoke(ctx);
		}

		protected virtual void OnObjectContainerGetted(IObjectContainerContext ctx) {
			this.ObjectContainerGetted?.Invoke(ctx);
		}

		protected virtual void OnObjectCreating(IObjectBuilderContext ctx) {
			this.ObjectCreating?.Invoke(ctx);
		}

		protected virtual void OnObjectCreated(IObjectBuilderContext ctx) {
			this.ObjectCreated?.Invoke(ctx);
		}
	}
}

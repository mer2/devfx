/* Copyright(c) 2005-2011 R2@DevFx.NET, License(LGPL) */

namespace HTB.DevFx.Data
{
	public abstract class ResultModuleBase : IResultModule
	{
		protected virtual void OnResultExecute(IDbResultContext ctx) {
			if(ctx.ResultHandled) {
				return;
			}
			this.OnResultExecuteInternal(ctx);
		}

		protected abstract void OnResultExecuteInternal(IDbResultContext ctx);

		#region IResultModule Members

		void IResultModule.OnResultExecute(IDbResultContext ctx) {
			this.OnResultExecute(ctx);
		}

		#endregion
	}
}

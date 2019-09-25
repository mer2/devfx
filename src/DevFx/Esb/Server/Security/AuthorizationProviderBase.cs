namespace DevFx.Esb.Server.Security
{
	public abstract class AuthorizationProviderBase : IAuthorizationProvider
	{
		public virtual bool Authorize(ServiceContext ctx, IAuthorizationIdentity identity) {
			var thisType = this.GetType();
			var validated = ctx.Items[thisType] as bool?;//避免重复验证
			if(validated != null) {
				return validated.Value;
			}
			var authorized = this.AuthorizeInternal(ctx, identity);
			if(!authorized && !ctx.ResultInitialized) {
				ctx.ResultInitialized = true;
				ctx.ResultValue = AOPResult.Failed(-1, "授权失败");
			}
			ctx.Items[thisType] = authorized;
			return authorized;
		}

		protected abstract bool AuthorizeInternal(ServiceContext ctx, IAuthorizationIdentity identity);
	}
}
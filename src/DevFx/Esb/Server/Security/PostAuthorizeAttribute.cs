using System;

namespace DevFx.Esb.Server.Security
{
	/// <summary>
	/// 限制请求只能以POST方式
	/// </summary>
	[Serializable, AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface | AttributeTargets.Method, AllowMultiple = false)]
	public class PostAuthorizeAttribute : AuthorizeAttribute
	{
		public const string AuthorizeCategory = nameof(PostAuthorizationProvider);
		public PostAuthorizeAttribute() : this("*") {
		}

		public PostAuthorizeAttribute(string user) : base(user) {
			this.Category = AuthorizeCategory;
		}

		public static bool Authorize(ServiceContext ctx, IAuthorizationIdentity identity) {
#if !DEBUG
			if(ctx.HttpContext.Request.Method != "POST") {
				ctx.ResultInitialized = true;
				ctx.ResultValue = AOPResult.Failed(-405, "非法请求");
				return false;
			}
#endif
			return true;
		}
	}

	[Object, AuthorizationProvider(nameof(PostAuthorizationProvider))]
	internal class PostAuthorizationProvider : AuthorizationProviderBase
	{
		protected override bool AuthorizeInternal(ServiceContext ctx, IAuthorizationIdentity identity) {
			return PostAuthorizeAttribute.Authorize(ctx, identity);
		}
	}
}

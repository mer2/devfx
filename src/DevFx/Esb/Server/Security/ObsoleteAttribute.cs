using System;

namespace DevFx.Esb.Server.Security
{
	/// <summary>
	/// 接口方法过期标记
	/// </summary>
	[Serializable, AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface | AttributeTargets.Method, AllowMultiple = false)]
	public class ObsoleteAttribute : AuthorizeAttribute
	{
		public const string AuthorizeCategory = nameof(ObsoleteAuthorizationProvider);
		public ObsoleteAttribute() : base("*") {
			this.Category = AuthorizeCategory;
		}
		public ObsoleteAttribute(string message) : this() {
			this.Message = message;
		}

		public string Message { get; set; }
	}

	[Object, AuthorizationProvider(nameof(ObsoleteAuthorizationProvider))]
	internal class ObsoleteAuthorizationProvider : AuthorizationProviderBase
	{
		protected override bool AuthorizeInternal(ServiceContext ctx, IAuthorizationIdentity identity) {
			string msg = null;
			var oa = (ObsoleteAttribute)identity;
			if (oa != null) {
				msg = oa.Message;
			}
			if (string.IsNullOrEmpty(msg)) {
				msg = "此方法已废弃，请更新客户端";
			}
			ctx.ResultInitialized = true;
			ctx.ResultValue = AOPResult.Failed(-304, msg);
			return false;
		}
	}
}

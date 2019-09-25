using System;

namespace DevFx.Esb.Server.Security
{
	[Serializable, AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface | AttributeTargets.Method, AllowMultiple = false)]
	public class NoneAuthorizeAttribute : AuthorizeAttribute
	{
		public const string AuthorizeCategory = "Application";

		public NoneAuthorizeAttribute() : base("*") {
			this.Category = AuthorizeCategory;
		}
	}
}
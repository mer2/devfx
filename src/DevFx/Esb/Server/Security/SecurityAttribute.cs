using System;

namespace DevFx.Esb.Server.Security
{
	[Serializable, AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface | AttributeTargets.Method, AllowMultiple = false)]
	public class SecurityAttribute : AuthorizeAttribute
	{
		public const string AuthorizeCategory = "Application";

		public SecurityAttribute() : base("?") {
			this.Category = AuthorizeCategory;
		}
	}
}
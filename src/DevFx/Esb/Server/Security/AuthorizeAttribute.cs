using System;

namespace DevFx.Esb.Server.Security
{
	[Serializable, AttributeUsage(AttributeTargets.Class | AttributeTargets.Method | AttributeTargets.Interface, Inherited = true, AllowMultiple = true)]
	public class AuthorizeAttribute : ActionFilterAttribute, IAuthorizationIdentity
	{
		public AuthorizeAttribute() : this("?", null) {}

		public AuthorizeAttribute(string users) : this(users, null) {
		}

		public AuthorizeAttribute(string users, string roles) : this(users, roles, "User") {
		}

		public AuthorizeAttribute(string users, string roles, string category) : base("Calling", category) {
			this.Users = users;
			this.Roles = roles;
		}
		
		public string Users { get; set; }
		public string Roles { get; set; }
	}
}

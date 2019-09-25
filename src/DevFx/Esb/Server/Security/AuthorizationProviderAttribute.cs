using System;
using DevFx.Core;

namespace DevFx.Esb.Server.Security
{
	[Serializable, AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
	public class AuthorizationProviderAttribute : CoreAttribute
	{
		public AuthorizationProviderAttribute(string category) {
			if(string.IsNullOrEmpty(category)) {
				throw new ArgumentNullException(nameof(category));
			}
			this.Category = category;
		}

		public string Category { get; set; }
	}
}
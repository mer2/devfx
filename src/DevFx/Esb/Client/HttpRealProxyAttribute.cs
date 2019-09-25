using DevFx.Core;
using System;

namespace DevFx.Esb.Client
{
	[Serializable, AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
	public class HttpRealProxyAttribute : CoreAttribute
	{
		public HttpRealProxyAttribute(string name) {
			if(string.IsNullOrEmpty(name)) {
				throw new ArgumentNullException(nameof(name));
			}
			this.Name = name;
		}
	}
}
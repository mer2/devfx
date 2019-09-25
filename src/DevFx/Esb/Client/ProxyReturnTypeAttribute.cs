using DevFx.Core;
using System;

namespace DevFx.Esb.Client
{
	[Serializable, AttributeUsage(AttributeTargets.Class | AttributeTargets.Assembly | AttributeTargets.Interface, AllowMultiple = true)]
	public class ProxyReturnTypeAttribute : CoreAttribute
	{
		public ProxyReturnTypeAttribute(string name) {
			if(string.IsNullOrEmpty(name)) {
				throw new ArgumentNullException(nameof(name));
			}

			this.Name = name;
		}
	}
}

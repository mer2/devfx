using DevFx.Core;
using System;

namespace DevFx.Esb.Server
{
	[Serializable, AttributeUsage(AttributeTargets.Interface | AttributeTargets.Assembly, AllowMultiple = true)]
	public class ServiceExportAttribute : CoreAttribute
	{
		public ServiceExportAttribute(string name) {
			if(string.IsNullOrEmpty(name)) throw new ArgumentNullException(nameof(name));
			this.Name = name;
		}

		public string AliasName { get; set; }
		public string Authorization { get; set; }
		public bool Inherits { get; set; }

		public Type ServiceType { get; set; }
	}
}

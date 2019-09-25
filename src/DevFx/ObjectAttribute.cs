using System;
using DevFx.Core;

namespace DevFx
{
	[Serializable, AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface, Inherited = false)]
	public sealed class ObjectAttribute : CoreAttribute
	{
		public string Lifetime { get; set; }
		public string Builder { get; set; }
	}
}
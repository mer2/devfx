using System;

namespace DevFx
{
	[Serializable, AttributeUsage(AttributeTargets.Property | AttributeTargets.Constructor)]
	public class AutowiredAttribute : Attribute
	{
		/// <summary>
		/// 确保要被注入
		/// </summary>
		public bool Required { get; set; }
	}
}
using System;

namespace DevFx.Core
{
	[Serializable, AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
	public class ObjectBuilderAttribute : CoreAttribute
	{
		public ObjectBuilderAttribute(string name) {
			this.Name = name;
		}
	}
}

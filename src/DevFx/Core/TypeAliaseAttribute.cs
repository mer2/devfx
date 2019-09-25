using System;

namespace DevFx.Core
{
	[Serializable, AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface | AttributeTargets.Assembly, AllowMultiple = true, Inherited = false)]
	public class TypeAliaseAttribute : CoreAttribute
	{
		public TypeAliaseAttribute(string name) {
			if (string.IsNullOrEmpty(name)) {
				throw new ArgumentNullException(nameof(name));
			}
			this.Name = name;
		}
	}
}

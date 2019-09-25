using System;

namespace DevFx.Core
{
	[Serializable, AttributeUsage(AttributeTargets.Assembly, AllowMultiple = true)]
	public class RemoveAttribute : Attribute
	{
		public RemoveAttribute(Type attributeType, string name) {
			if (string.IsNullOrEmpty(name)) {
				throw new ArgumentNullException(nameof(name));
			}
			this.AttributeType = attributeType ?? throw new ArgumentNullException(nameof(attributeType));
			this.Name = name;
		}
		public RemoveAttribute(Type attributeType, Type ownerType) {
			this.AttributeType = attributeType ?? throw new ArgumentNullException(nameof(attributeType));
			this.OwnerType = ownerType ?? throw new ArgumentNullException(nameof(ownerType));
		}

		public Type AttributeType { get; set; }
		public string Name { get; set; }
		public Type OwnerType { get; set; }
	}
}

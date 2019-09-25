using System;

namespace DevFx.Configuration
{
	[Serializable, AttributeUsage(AttributeTargets.Property)]
	public class SettingNameAttribute : Attribute
	{
		public SettingNameAttribute(string name) {
			if(string.IsNullOrEmpty(name)) {
				throw new ArgumentException(nameof(name));
			}
			this.Name = name;
		}

		public string Name { get; set; }
	}
}

namespace DevFx.Configuration
{
	public class NameSetting : ConfigSettingElement, INameSetting
	{
		protected override void OnConfigSettingChanged() {
			base.OnConfigSettingChanged();
			this.Name = this.GetSetting<string>("name");
		}

		public string Name { get; private set; }
	}

	public class TypeSetting : NameSetting, ITypeSetting
	{
		protected override void OnConfigSettingChanged() {
			base.OnConfigSettingChanged();
			this.TypeName = this.GetSetting<string>("type");
		}

		public string TypeName { get; private set; }
	}

	public class ValueSetting : TypeSetting, IValueSetting
	{
		protected override void OnConfigSettingChanged() {
			base.OnConfigSettingChanged();
			var value = this.GetSetting<string>("value");
			if (value == null) {
				value = this.ConfigSetting.Value.Value;
			}
			this.Value = value;
		}

		public string Value { get; private set; }
	}
}

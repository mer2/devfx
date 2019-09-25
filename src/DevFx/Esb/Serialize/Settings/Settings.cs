using DevFx.Configuration;
[assembly: ConfigResource("res://DevFx.Esb.Serialize.Settings.Settings.config", Index = 2000)]

namespace DevFx.Esb.Serialize.Settings
{
	[SettingObject("~/esb/serialize", Required = true)]
	internal class SerializerFactorySetting : ConfigSettingElement
	{
		protected override void OnConfigSettingChanged() {
			base.OnConfigSettingChanged();
			this.DefaultSerializerName = this.GetSetting<string>("defaultSerializer", required:true);
			this.Debug = this.GetSetting("debug", false);
			this.Serializers = this.GetSettings<SerializerSetting>(null).ToArray();
		}

		public string DefaultSerializerName { get; private set; }
		public bool Debug { get; private set; }
		public SerializerSetting[] Serializers { get; private set; }
	}

	internal class SerializerSetting : ConfigSettingElement
	{
		protected override void OnConfigSettingChanged() {
			base.OnConfigSettingChanged();
			this.Name = this.GetSetting<string>("name", required: true);
			this.ContentType = this.GetSetting<string>("contentType", required: true);
			this.TypeName = this.GetSetting<string>("type", required: true);
			this.Enabled = this.GetSetting("enabled", true);
		}

		public string Name { get; private set; }
		public string ContentType { get; private set; }
		public string TypeName { get; private set; }
		public bool Enabled { get; private set; }
	}
}

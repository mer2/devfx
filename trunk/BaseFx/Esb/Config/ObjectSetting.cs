/* Copyright(c) 2005-2011 R2@DevFx.NET, License(LGPL) */

using System.Collections;
using System.Collections.Specialized;
using HTB.DevFx.Config;
using HTB.DevFx.Core.Config;
using HTB.DevFx.Utils;

namespace HTB.DevFx.Esb.Config
{
	internal class ObjectSetting : ConfigSettingElement, IObjectSettingInternal
	{
		protected override void OnConfigSettingChanged() {
			base.OnConfigSettingChanged();
			this.Name = this.GetSetting("name");
			this.TypeName = this.GetSetting("type");
			this.MapTo = this.GetSetting("mapTo");
			this.Builder = this.GetSetting("builder");
		}

		public string Name { get; private set; }
		public string TypeName { get; private set; }
		public string MapTo { get; private set; }
		public string Builder { get; private set; }

		private SettingSetInternal settingSet;
		public IValueSetting SettingSet {
			get {
				if (this.settingSet == null) {
					var settingString = this.GetSetting("setting");
					this.settingSet = SettingSetInternal.Create(settingString);
				}
				return this.settingSet;
			}
		}

		public object ObjectInstance { get; set; }

		#region Implementation of IObjectContext

		private IDictionary items;
		public IDictionary Items {
			get { return this.items ?? (this.items = new HybridDictionary()); }
		}

		#endregion
	}

	internal class TypeSetting : ConfigSettingElement, ITypeSetting
	{
		protected override void OnConfigSettingChanged() {
			base.OnConfigSettingChanged();
			this.Name = this.GetSetting("name");
			this.TypeName = this.GetSetting("type");
		}

		public string Name { get; private set; }
		public string TypeName { get; private set; }
	}

	internal class SettingSetInternal : IValueSetting
	{
		public string Name { get; set; }
		public string Type { get; set; }
		public string Value { get; set; }

		public static SettingSetInternal Create(string settingSet) {
			SettingSetInternal instance = null;
			if (!string.IsNullOrEmpty(settingSet)) {
				instance = WebHelper.FromJsonLite<SettingSetInternal>(settingSet, (c, n, v) => {
					switch(n.ToLower()) {
						case "name":
							c.Name = v;
							break;
						case "type":
							c.Type = v;
							break;
						case "value":
							c.Value = v;
							break;
					}
				});
			}
			return instance;
		}

		#region Implementation of ITypeSetting

		string ITypeSetting.TypeName {
			get { return this.Type; }
		}

		#endregion
	}
}
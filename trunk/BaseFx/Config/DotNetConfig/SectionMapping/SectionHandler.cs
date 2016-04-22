/* Copyright(c) 2005-2011 R2@DevFx.NET, License(LGPL) */

using System;
using System.Collections.Generic;
using System.Configuration;
using System.Xml;
using HTB.DevFx.Utils;

namespace HTB.DevFx.Config.DotNetConfig.SectionMapping
{
	[ConfigSectionNames("htb.devfx/sectionMap", "sectionMap")]
	internal class SectionHandler : IConfigurationSectionHandler, IRequiresEmptyInstance
	{
		public static SectionHandler Default {
			get {
				var list = new[] { "htb.devfx/sectionMap", "sectionMap" };
				return ConfigHelper.GetSectionFromConfiguration<SectionHandler>(true, list);
			}
		}

		internal Dictionary<string, Type> GetSections() {
			return this.currentSections;
		}

		private Dictionary<string, Type> currentSections;

		private static void BuildSections(IDictionary<string, Type> dict, string groupName, XmlNode section) {
			foreach (XmlNode child in section.ChildNodes) {
				if (XmlNodeType.Element == child.NodeType) {
					var cgn = groupName;
					if (child.Name == "section") {
						var sectionName = child.Attributes["name"].Value;
						var typeName = child.Attributes["type"].Value;
						if (string.IsNullOrEmpty(sectionName)) {
							throw new ConfigException("sectionName required");
						}
						if (string.IsNullOrEmpty(typeName)) {
							throw new ConfigException("type required");
						}
						var type = TypeHelper.CreateType(typeName, true);

						sectionName = cgn = string.IsNullOrEmpty(cgn) ? sectionName : cgn + "/" + sectionName;
						if (dict.ContainsKey(sectionName)) {
							dict[sectionName] = type;
						} else {
							dict.Add(sectionName, type);
						}
					} else if (child.Name == "sectionGroup") {
						var gn = child.Attributes["name"] == null ? null : child.Attributes["name"].Value;
						if (string.IsNullOrEmpty(gn)) {
							gn = cgn;
						}
						if (!string.IsNullOrEmpty(cgn)) {
							gn = cgn + "/" + gn;
						}
						cgn = gn;
					} else {
						continue;
					}
					BuildSections(dict, cgn, child);
				}
			}
		}

		#region IConfigurationSectionHandler

		object IConfigurationSectionHandler.Create(object parent, object configContext, XmlNode section) {
			var parentHandler = (SectionHandler)parent;
			var dict = parentHandler == null ? new Dictionary<string, Type>() : new Dictionary<string, Type>(parentHandler.currentSections);
			BuildSections(dict, null, section);
			return new SectionHandler {
				currentSections = dict
			};
		}

		#endregion IConfigurationSectionHandler
	}
}
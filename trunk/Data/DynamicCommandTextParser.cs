/* Copyright(c) 2005-2011 R2@DevFx.NET, License(LGPL) */

using System.Collections;
using System.Text;
using HTB.DevFx.Config;
using HTB.DevFx.Data.Config;
using HTB.DevFx.Utils;

namespace HTB.DevFx.Data
{
	internal class DynamicCommandTextParser : CommandTextParser
	{
		protected DynamicCommandTextParser() { }

		protected override string GetCommandText(IStatementSetting statement, IDictionary parameters) {
			var config = statement.StatementText.ConfigSetting;
			return this.GetCommandText(statement.GetObjectContext(this, () => config.ToSetting<DynamicTextSetting>()), parameters);
		}

		internal string GetCommandText(IDynamicTextSetting dynamicText, IDictionary parameters) {
			var parameterName = dynamicText.ParameterName;
			if(!string.IsNullOrEmpty(parameterName)) {
				var isPresent = dynamicText.IsPresent;
				var isNull = dynamicText.IsNull;
				if(isPresent.HasValue) {
					if ((isPresent.Value && !parameters.Contains(parameterName)) || (!isPresent.Value && parameters.Contains(parameterName))) {
						return string.Empty;
					}
				}
				if(isNull.HasValue) {
					var value = parameters[parameterName];
					if((isNull.Value && value != null) || (!isNull.Value && value == null)) {
						return string.Empty;
					}
				}
			}
			var values = dynamicText.ConfigSetting.Value.Values;
			if(values.Length <= 1) {
				return dynamicText.ConfigSetting.Value.Value;
			}
			var text = new StringBuilder();
			var texts = dynamicText.DynamicTexts;
			var valueIndex = 0;
			foreach(var t in values) {
				var value = t;
				if (value is int) {
					if (valueIndex < texts.Length) {
						text.Append(this.GetCommandText(texts[valueIndex++], parameters));
					}
				} else {
					text.Append(t);
				}
			}
			return text.ToString();
		}
	}
}

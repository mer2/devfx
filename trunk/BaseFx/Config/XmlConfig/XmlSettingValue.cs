/* Copyright(c) 2005-2011 R2@DevFx.NET, License(LGPL) */

using System.Collections.Generic;
using System.Text;
using System.Web;
using System.Xml;

namespace HTB.DevFx.Config.XmlConfig
{
	/// <summary>
	/// 使用XML实现<see cref="SettingValue"/>
	/// </summary>
	public class XmlSettingValue : SettingValue
	{
		#region constructor

		/// <summary>
		/// 使用name/value的形式初始化
		/// </summary>
		/// <param name="name">配置值名</param>
		/// <param name="value">配置值</param>
		/// <param name="readonly">是否只读</param>
		/// <param name="values">多值</param>
		public XmlSettingValue(string name, string @value, bool @readonly, object[] values) : base(name, @value, @readonly, values) { }

		/// <summary>
		/// 使用XmlNode初始化
		/// </summary>
		/// <param name="xmlNode">XmlNode</param>
		/// <param name="readonly">是否只读</param>
		public XmlSettingValue(XmlNode xmlNode, bool @readonly) : base(null, null, @readonly, null) {
			this.name = xmlNode.Name;
			if(xmlNode.ChildNodes.Count <= 0) {
				return;
			}
			var values = new List<object>();
			var valueFormat = new StringBuilder();
			var valueExists = false;
			var valueIndex = 0;
			foreach(XmlNode node in xmlNode.ChildNodes) {
				switch(node.NodeType) {
					case XmlNodeType.Text:
					case XmlNodeType.CDATA:
						values.Add(node.Value);
						valueFormat.Append(node.Value);
						valueExists = true;
						break;
					case XmlNodeType.Element:
						values.Add(valueIndex++);
						break;
				}
			}
			this.values = values.ToArray();
			if (valueExists) {
				this.value = valueFormat.ToString();
			}
		}

		#endregion

		/// <summary>
		/// 创建配置值实例
		/// </summary>
		/// <param name="name">配置值名</param>
		/// <param name="value">配置值</param>
		/// <param name="readonly">是否只读</param>
		/// <returns>SettingValue</returns>
		/// <param name="values">多值</param>
		protected override SettingValue CreateSettingValue(string name, string value, bool @readonly, object[] values) {
			return new XmlSettingValue(name, value, @readonly, values);
		}

		/// <summary>
		/// 转换成字符串格式
		/// </summary>
		/// <returns>字符串</returns>
		public override string ToString() {
			return string.Format("{0}=\"{1}\"", this.Name, HttpUtility.HtmlAttributeEncode(this.Value));
		}
	}
}
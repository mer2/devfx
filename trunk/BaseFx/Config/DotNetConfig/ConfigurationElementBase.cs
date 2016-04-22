/* Copyright(c) 2005-2011 R2@DevFx.NET, License(LGPL) */

using System;
using System.Collections.Specialized;
using System.Configuration;
using System.Xml;
using HTB.DevFx.Data;

namespace HTB.DevFx.Config.DotNetConfig
{
	/// <summary>
	/// 配置元素基础类，从<see cref="ConfigurationElement"/>派生
	/// </summary>
	/// <remarks>
	///		<para>此类对基类<see cref="ConfigurationElement"/>做了些修改，允许未定义的属性存在</para>
	///		<para>派生类如果要允许未定义的属性，则必须重写<see cref="ConfigurationElementBase.OnDeserializeUnrecognizedFlag"/></para>
	/// </remarks>
	public abstract class ConfigurationElementBase : ConfigurationElement
	{
		/// <summary>
		/// 获取一个值，该值指示反序列化过程中是否遇到未知属性
		/// </summary>
		/// <param name="name">无法识别的属性的名称</param>
		/// <param name="value">无法识别的属性的值</param>
		/// <returns>如果反序列化过程中遇到未知属性，则为<c>true</c></returns>
		protected override bool OnDeserializeUnrecognizedAttribute(string name, string value) {
			this.unkownProperties.Add(name, value);
			return this.OnDeserializeUnrecognizedFlag;
		}

		/// <summary>
		/// 获取一个值，该值指示反序列化过程中是否遇到未知元素
		/// </summary>
		/// <param name="elementName">未知的子元素的名称</param>
		/// <param name="reader">用于反序列化的 <seealso cref="XmlReader"/> 对象</param>
		/// <returns>如果反序列化过程中遇到未知元素，则为 true</returns>
		protected override bool OnDeserializeUnrecognizedElement(string elementName, XmlReader reader) {
			return this.OnDeserializeUnrecognizedFlag;
		}

		/// <summary>
		/// 是否遇到未知的属性或元素
		/// </summary>
		/// <remarks>
		///		<para>派生类如果要允许未定义的属性，则必须重写本属性</para>
		/// </remarks>
		protected virtual bool OnDeserializeUnrecognizedFlag {
			get { return false; }
		}

		/// <summary>
		/// 本元素对应的Xml
		/// </summary>
		public virtual string OuterXml {
			get { return this.outerXml; }
		}

		private string outerXml;
		private readonly NameValueCollection unkownProperties = new NameValueCollection();

		/// <summary>
		/// 读取配置文件中的 XML
		/// </summary>
		/// <param name="reader">在配置文件中进行读取操作的 <seealso cref="XmlReader"/></param>
		/// <param name="serializeCollectionKey">为 <c>true</c>，则只序列化集合的键属性；否则为 <c>false</c></param>
		protected override void DeserializeElement(XmlReader reader, bool serializeCollectionKey) {
			var field = reader.GetType().GetField("_rawXml", FieldMemberInfo.FieldBindingFlags);
			this.outerXml = (string)field.GetValue(reader);
			base.DeserializeElement(reader, serializeCollectionKey);
		}

		/// <summary>
		/// 获取未定义的属性值
		/// </summary>
		/// <param name="propertyName">属性名</param>
		/// <returns>属性值</returns>
		public virtual string GetPropertyValue(string propertyName) {
			return this.unkownProperties[propertyName];
		}

		/// <summary>
		/// 获取未定义的属性值
		/// </summary>
		/// <typeparam name="T">属性值类型</typeparam>
		/// <param name="propertyName">属性名</param>
		/// <returns>属性值</returns>
		public virtual T GetPropertyValue<T>(string propertyName) {
			return this.GetPropertyValue(propertyName, default(T));
		}

		/// <summary>
		/// 获取未定义的属性值
		/// </summary>
		/// <typeparam name="T">属性值类型</typeparam>
		/// <param name="propertyName">属性名</param>
		/// <param name="defaultValue">如果此属性不存在需提供的缺省值</param>
		/// <returns>属性值</returns>
		public virtual T GetPropertyValue<T>(string propertyName, T defaultValue) {
			var propertyValue = this.GetPropertyValue(propertyName);
			var returnValue = defaultValue;
			if (!string.IsNullOrEmpty(propertyValue)) {
				returnValue = (T)Convert.ChangeType(propertyValue, typeof(T));
			}
			return returnValue;
		}

		/// <summary>
		/// 检测指定的属性是否存在
		/// </summary>
		/// <param name="propertyName">属性名</param>
		/// <returns>是否存在</returns>
		public virtual bool CheckPropertyExists(string propertyName) {
			return this.unkownProperties[propertyName] != null;
		}
	}
}
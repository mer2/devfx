/* Copyright(c) 2005-2011 R2@DevFx.NET, License(LGPL) */

using System.Collections.Generic;
using System.Configuration;
using System.Reflection;
using System.Web.Configuration;
using System.Xml;
using HTB.DevFx.Config.DotNetConfig.SectionMapping;
using HTB.DevFx.Data;
using HTB.DevFx.Utils;

namespace HTB.DevFx.Config.DotNetConfig
{
	/// <summary>
	/// 配置节基础处理类，继承自 <see cref="ConfigurationSection"/>
	/// </summary>
	/// <typeparam name="T">派生类</typeparam>
	/// <remarks>
	///		<para>
	///			注意与 <see cref="ConfigurationElementBase"/> 的区别
	///		</para>
	///		<para>
	///			获取当前缺省实例，请使用<see cref="Current"/>方法，将按下列顺序查找配置节实例
	///			<list>如果配置了配置节映射，则从映射中查找</list>
	///			<list>如果配置节定义了<see cref="ConfigSectionNamesAttribute"/>属性，则按此属性的定义获取</list>
	///			<list>否则按<see cref="GroupHandler"/>的处理方式来处理，请尽量避免使用这种方法来配置</list>
	///		</para>
	/// </remarks>
	public abstract class SectionHandlerBase<T> : ConfigurationSection where T : SectionHandlerBase<T>
	{
		/// <summary>
		/// 获取当前配置节实例
		/// </summary>
		public static T Current {
			get {
				var sh = GetSectionInternal(null, false, false) ?? GroupHandler.GetSection<T>(false);
				if (sh == null) {
					var t = typeof(T);
					if(typeof(IRequiresEmptyInstance).IsAssignableFrom(t)) {
						sh = (T)TypeHelper.CreateObject(t, t, true);
					}
				}
				return sh;
			}
		}

		/// <summary>
		/// 按配置节名称获取当前配置节实例
		/// </summary>
		/// <param name="sectionName">配置节名，如果为<c>null</c>，则将从配置映射和配置节属性中查找配置节名</param>
		/// <returns>如果未找到配置节，且配置节未实现<see cref="IRequiresEmptyInstance"/>则返回空</returns>
		public static T GetSection(string sectionName) {
			return GetSectionInternal(sectionName, true, false);
		}

		/// <summary>
		/// 直接从<see cref="ConfigurationManager"/>或<see cref="WebConfigurationManager"/>获得配置节实例
		/// </summary>
		/// <param name="sectionName">配置节名称</param>
		/// <param name="createDefault">如果配置节不存在是否返回缺省实例</param>
		/// <returns>配置节实例</returns>
		public static T GetSectionFromConfiguration(string sectionName, bool createDefault) {
			return GetSectionFromConfiguration(new [] { sectionName }, createDefault);
		}

		private static T GetSectionFromConfiguration(string[] sectionNames, bool createDefault) {
			return ConfigHelper.GetSectionFromConfiguration<T>(createDefault, sectionNames);
		}

		internal static T GetSectionInternal(string sectionName, bool createDefault, bool onlyFromAttribute) {
			var nameList = GetSectionNames(typeof(T), onlyFromAttribute);
			if (!string.IsNullOrEmpty(sectionName)) {
				nameList.Insert(0, sectionName);
			}
			return GetSectionFromConfiguration(nameList.ToArray(), createDefault);
		}

		private static List<string> GetSectionNames(ICustomAttributeProvider type, bool onlyFromAttribute) {
			var nameList = new List<string>();
			if(!onlyFromAttribute) {
				var mappings = SectionHandler.Default.GetSections();
				foreach (var mapElement in mappings) {
					if (mapElement.Value == type) {
						nameList.Add(mapElement.Key);
					}
				}
			}
			nameList.AddRange(ConfigSectionNamesAttribute.GetSectionNames(type));
			return nameList;
		}

		/// <summary>
		/// 获取一个值，该值指示反序列化过程中是否遇到未知属性
		/// </summary>
		/// <param name="name">无法识别的属性的名称</param>
		/// <param name="value">无法识别的属性的值</param>
		/// <returns>如果反序列化过程中遇到未知属性，则为<c>true</c></returns>
		protected override bool OnDeserializeUnrecognizedAttribute(string name, string value) {
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
		///  是否允许未知的属性或元素
		/// </summary>
		/// <remarks>
		///		<para>派生类如果要允许未定义的属性，则必须重写本属性</para>
		/// </remarks>
		protected virtual bool OnDeserializeUnrecognizedFlag {
			get { return false; }
		}

		/// <summary>
		/// 本配置节对应的Xml
		/// </summary>
		public virtual string OuterXml {
			get { return this.outerXml; }
		}

		private string outerXml;

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
	}
}
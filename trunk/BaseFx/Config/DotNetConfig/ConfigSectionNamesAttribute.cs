/* Copyright(c) 2005-2011 R2@DevFx.NET, License(LGPL) */

using System;
using System.Collections.Generic;
using System.Reflection;

namespace HTB.DevFx.Config.DotNetConfig
{
	/// <summary>
	/// 配置节名称定义属性
	/// </summary>
	[AttributeUsage(AttributeTargets.Class, Inherited = true, AllowMultiple = false)]
	public sealed class ConfigSectionNamesAttribute : Attribute
	{
		/// <summary>
		/// 构造方法
		/// </summary>
		/// <param name="name">配置节名</param>
		/// <param name="names">备查的其他节名</param>
		public ConfigSectionNamesAttribute(string name, params string[] names) {
			if(string.IsNullOrEmpty(name)) {
				throw new ArgumentNullException("name");
			}
			var nameList = new List<string> { name };
			if(names != null && names.Length > 0) {
				foreach(var n in names) {
					if(!string.IsNullOrEmpty(n)) {
						nameList.Add(n);
					}
				}
			}
			this.Names = nameList.ToArray();
		}

		/// <summary>
		/// 获取配置节名称列表
		/// </summary>
		public string[] Names { get; private set; }

		internal static List<string> GetSectionNames(ICustomAttributeProvider type) {
			var nameList = new List<string>();
			var attriubtes = type.GetCustomAttributes(typeof(ConfigSectionNamesAttribute), true);
			if (attriubtes != null && attriubtes.Length > 0) {
				var names = ((ConfigSectionNamesAttribute)attriubtes[0]).Names;
				nameList.AddRange(names);
			}
			return nameList;
		}
	}
}

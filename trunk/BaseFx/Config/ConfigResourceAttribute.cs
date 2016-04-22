/* Copyright(c) 2005-2011 R2@DevFx.NET, License(LGPL) */

using System;
using System.Reflection;

namespace HTB.DevFx.Config
{
	/// <summary>
	/// 配置文件资源属性
	/// </summary>
	/// <remarks>
	/// 这个提供配置文件合并的一种方式<br />
	/// 用法，在应用程序中添加如下元属性
	///		<code>
	///			[assembly: ConfigResource("res://HTB.DevFx.Config.htb.devfx.config", Index = 100)]
	///		</code>
	/// </remarks>
	[AttributeUsage(AttributeTargets.Assembly, AllowMultiple = true)]
	public sealed class ConfigResourceAttribute : Attribute
	{
		/// <summary>
		/// 构造函数
		/// </summary>
		/// <param name="resource">指定配置文件地址</param>
		public ConfigResourceAttribute(string resource) {
			this.Resource = resource;
		}

		/// <summary>
		/// 配置文件地址
		/// </summary>
		public string Resource { get; set; }

		/// <summary>
		/// 合并顺序
		/// </summary>
		public int Index { get; set; }

		/// <summary>
		/// 资源文件所在的程序集名（内部使用）
		/// </summary>
		internal string AssemblyName { get; set; }

		/// <summary>
		/// 资源文件所在的程序集（内部使用）
		/// </summary>
		internal Assembly Assembly { get; set; }
	}
}

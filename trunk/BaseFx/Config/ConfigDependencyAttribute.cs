/* Copyright(c) 2005-2012 R2@DevFx.NET, License(LGPL) */

using System;
using System.Reflection;

namespace HTB.DevFx.Config
{
	/// <summary>
	/// 配置文件资源程序集依赖关系属性
	/// </summary>
	/// <remarks>
	/// 这个提供配置文件之间（无直接引用关系）依赖关系的一种方式<br />
	/// 用法，在应用程序中添加如下元属性
	///		<code>
	///			[assembly: ConfigDependency("HTB.DevFx")]
	///		</code>
	/// </remarks>
	[AttributeUsage(AttributeTargets.Assembly, AllowMultiple = true)]
	public sealed class ConfigDependencyAttribute : Attribute
	{
		/// <summary>
		/// 构造函数
		/// </summary>
		/// <param name="assemblyName">指定程序集名称</param>
		public ConfigDependencyAttribute(string assemblyName) {
			this.AssemblyName = assemblyName;
		}

		/// <summary>
		/// 程序集名
		/// </summary>
		public string AssemblyName { get; set; }

		/// <summary>
		/// 合并顺序
		/// </summary>
		public int Index { get; set; }

		/// <summary>
		/// 程序集（内部使用）
		/// </summary>
		internal Assembly Assembly { get; set; }
	}
}

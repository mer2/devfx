using System;
using System.Reflection;

namespace DevFx.Reflection
{
	/// <summary>
	/// 程序集依赖关系属性
	/// </summary>
	/// <remarks>
	/// 这个提供程序集之间（无直接引用关系）依赖关系的一种方式<br />
	/// 用法，在应用程序中添加如下元属性
	///		<code>
	///			[assembly: AssemblyDependency("DevFx")]//DevFx为程序集名称
	///		</code>
	/// </remarks>
	[AttributeUsage(AttributeTargets.Assembly, AllowMultiple = true)]
	public sealed class AssemblyDependencyAttribute : Attribute
	{
		/// <summary>
		/// 构造函数
		/// </summary>
		/// <param name="assemblyName">指定程序集名称</param>
		public AssemblyDependencyAttribute(string assemblyName) {
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

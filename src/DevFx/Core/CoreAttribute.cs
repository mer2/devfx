using System;

namespace DevFx.Core
{
	[Serializable]
	public abstract class CoreAttribute : Attribute
	{
		/// <summary>
		/// 别名
		/// </summary>
		public string Name { get; set; }
		//被应用的类型
		public Type OwnerType { get; set; }
		/// <summary>
		/// 优先级，数字越大优先级越高
		/// </summary>
		public int Priority { get; set; }
	}
}
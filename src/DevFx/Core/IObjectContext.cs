using System.Collections;

namespace DevFx.Core
{
	/// <summary>
	/// 对象上下文接口
	/// </summary>
	/// <remarks>
	/// 用于在获取或创建对象时传递的上下文
	/// </remarks>
	public interface IObjectContext
	{
		/// <summary>
		/// 传递的上下文
		/// </summary>
		IDictionary Items { get; }
	}
}

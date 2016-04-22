/* Copyright(c) 2005-2011 R2@DevFx.NET, License(LGPL) */

using HTB.DevFx.Core;
using HTB.DevFx.Esb.Config;

namespace HTB.DevFx.Esb
{
	/// <summary>
	/// 对象创建的上下文
	/// </summary>
	public interface IObjectBuildContext : IObjectContext
	{
		/// <summary>
		/// 当前<see cref="IServiceLocator"/>实例
		/// </summary>
		IServiceLocator ServiceLocator { get; }
		/// <summary>
		/// 当前<see cref="IObjectBuilder"/>实例
		/// </summary>
		IObjectBuilder ObjectBuilder { get; }
		/// <summary>
		/// 当前<see cref="IObjectSetting"/>实例
		/// </summary>
		IObjectSetting ObjectSetting { get; }
		/// <summary>
		/// 当前创建的对象实例
		/// </summary>
		object ObjectInstance { get; set; }
	}
}

/* Copyright(c) 2005-2011 R2@DevFx.NET, License(LGPL) */

using System.Collections;
using HTB.DevFx.Core;
using HTB.DevFx.Esb.Config;

namespace HTB.DevFx.Esb
{
	/// <summary>
	/// 对象创建的上下文
	/// </summary>
	internal class ObjectBuildContext : ObjectContextBase, IObjectBuildContext
	{
		/// <summary>
		/// 构造
		/// </summary>
		public ObjectBuildContext(IServiceLocator serviceLocator, IObjectSetting objectSetting, IObjectBuilder objectBuilder, IDictionary items) : base(items) {
			this.ServiceLocator = serviceLocator;
			this.ObjectSetting = objectSetting;
			this.ObjectBuilder = objectBuilder;
		}

		/// <summary>
		/// 当前<see cref="IServiceLocator"/>实例
		/// </summary>
		public IServiceLocator ServiceLocator { get; private set; }
		/// <summary>
		/// 当前<see cref="IObjectSetting"/>实例
		/// </summary>
		public IObjectSetting ObjectSetting { get; private set; }
		/// <summary>
		/// 当前<see cref="IObjectBuilder"/>实例
		/// </summary>
		public IObjectBuilder ObjectBuilder { get; private set; }
		/// <summary>
		/// 当前创建的对象实例
		/// </summary>
		public object ObjectInstance { get; set; }
	}
}

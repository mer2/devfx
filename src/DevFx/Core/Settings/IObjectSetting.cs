/* Copyright(c) 2005-2011 R2@DevFx.NET, License(LGPL) */

using DevFx.Configuration;

namespace DevFx.Core.Settings
{
	public interface IObjectSetting : ITypeSetting
	{
		/// <summary>
		/// 对象构造配置信息
		/// </summary>
		IValueSetting[] ConstructorParameters { get; }
		/// <summary>
		/// 对象注入的属性信息
		/// </summary>
		IValueSetting[] Properties { get; }
		/// <summary>
		/// 对象依赖信息
		/// </summary>
		INameSetting[] Dependencies { get; }

		/// <summary>
		/// 对象生命周期配置
		/// </summary>
		string Lifetime { get; }
		/// <summary>
		/// 对象创建器名
		/// </summary>
		string Builder { get; }
	}
}

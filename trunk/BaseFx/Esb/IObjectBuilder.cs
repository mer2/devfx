/* Copyright(c) 2005-2011 R2@DevFx.NET, License(LGPL) */

using HTB.DevFx.Esb.Config;

namespace HTB.DevFx.Esb
{
	///<summary>
	/// 对象创建器接口，用于各种对象的创建
	///</summary>
	public interface IObjectBuilder
	{
		///<summary>
		/// 根据给定参数，创建对象
		///</summary>
		///<param name="setting">对象配置信息</param>
		///<param name="parameters">参数列表</param>
		///<returns>被创建的对象实例</returns>
		object CreateObject(IObjectSetting setting, params object[] parameters);
	}
}
/* Copyright(c) 2005-2011 R2@DevFx.NET, License(LGPL) */

using System;
using HTB.DevFx.Esb.Config;

namespace HTB.DevFx.Remoting
{
	/// <summary>
	/// 远程对象创建器
	/// </summary>
	public interface IRemotingObjectBuilder
	{
		/// <summary>
		/// 创建远程对象
		/// </summary>
		/// <param name="setting">对象配置</param>
		/// <param name="objectType">对象类型</param>
		/// <param name="uri">远程地址</param>
		/// <param name="parameters">其他参数</param>
		/// <returns>远程对象</returns>
		object CreateObject(IObjectSetting setting, Type objectType, string uri, params object[] parameters);
	}
}
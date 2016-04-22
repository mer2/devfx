/* Copyright(c) 2005-2011 R2@DevFx.NET, License(LGPL) */

using System.Security.Permissions;

namespace HTB.DevFx.Security
{
	/// <summary>
	/// 拦截动作
	/// </summary>
	public enum InterceptorAction
	{
		/// <summary>
		/// 要求拦截认证
		/// </summary>
		Demand = SecurityAction.Demand
	}
}

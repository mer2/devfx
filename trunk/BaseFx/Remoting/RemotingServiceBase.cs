/* Copyright(c) 2005-2011 R2@DevFx.NET, License(LGPL) */

using System;
using System.Reflection;
using HTB.DevFx.Reflection;

namespace HTB.DevFx.Remoting
{
	/// <summary>
	/// 使用Remoting技术发布服务，动态生成服务代理的基类
	/// </summary>
	public abstract class RemotingServiceBase<TContract> : MarshalByRefObject, IObjectProxyBase<TContract>
	{
		/// <summary>
		/// 服务实例
		/// </summary>
		public TContract ProxyInstance { get; set; }

		/// <summary>
		/// 在服务方法被调用前被调用的方法
		/// </summary>
		/// <param name="ctx">方法调用上下文</param>
		public void BeforeCall(CallContext ctx) {
		}

		/// <summary>
		/// 在服务方法被调用后被调用的方法
		/// </summary>
		/// <param name="ctx">方法调用上下文</param>
		public void AfterCall(CallContext ctx) {
		}

		/// <summary>
		/// 在服务方法被调用时发生错误的处理方法
		/// </summary>
		/// <param name="ctx">方法调用上下文</param>
		public void OnException(CallContext ctx) {
		}

		/// <summary>
		/// 获取方法调用的上下文
		/// </summary>
		/// <param name="method">方法</param>
		/// <param name="args">调用参数</param>
		/// <returns>方法调用上下文</returns>
		public CallContext GetCallContext(MethodInfo method, params object[] args) {
			return new CallContext(this.ProxyInstance, method, args);
		}
	}
}

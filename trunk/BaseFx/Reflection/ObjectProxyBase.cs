/* Copyright(c) 2005-2011 R2@DevFx.NET, License(LGPL) */

using System.Reflection;

namespace HTB.DevFx.Reflection
{
	/// <summary>
	/// 实现动态创建类型的基类
	/// </summary>
	/// <typeparam name="TContract">服务接口</typeparam>
	public class ObjectProxyBase<TContract> : IObjectProxyBase<TContract>
	{
		/// <summary>
		/// 服务实例
		/// </summary>
		public virtual TContract ProxyInstance { get; set; }

		/// <summary>
		/// 在服务方法被调用前被调用的方法
		/// </summary>
		/// <param name="ctx">方法调用上下文</param>
		public virtual void BeforeCall(CallContext ctx) {
		}

		/// <summary>
		/// 在服务方法被调用后被调用的方法
		/// </summary>
		/// <param name="ctx">方法调用上下文</param>
		public virtual void AfterCall(CallContext ctx) {
		}

		/// <summary>
		/// 在服务方法被调用时发生错误的处理方法
		/// </summary>
		/// <param name="ctx">方法调用上下文</param>
		public virtual void OnException(CallContext ctx) {
		}

		/// <summary>
		/// 获取方法调用的上下文
		/// </summary>
		/// <param name="method">方法</param>
		/// <param name="args">调用参数</param>
		/// <returns>方法调用上下文</returns>
		public virtual CallContext GetCallContext(MethodInfo method, params object[] args) {
			return new CallContext(this.ProxyInstance, method, args);
		}
	}
}

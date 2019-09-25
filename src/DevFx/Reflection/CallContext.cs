/* Copyright(c) 2005-2011 R2@DevFx.NET, License(LGPL) */

using System;
using System.Collections;
using System.Collections.Specialized;
using System.Reflection;

namespace DevFx.Reflection
{
	/// <summary>
	/// 方法调用上下文
	/// </summary>
	public class CallContext
	{
		/// <summary>
		/// 构造方法
		/// </summary>
		/// <param name="instance">被调用的对象实例</param>
		/// <param name="method">方法</param>
		/// <param name="args">调用参数</param>
		public CallContext(object instance, MethodInfo method, object[] args) {
			this.ObjectInstance = instance;
			this.CallMethod = method;
			this.CallArguments = args;
		}

		/// <inheritdoc />
		/// <summary>
		/// 构造方法
		/// </summary>
		/// <param name="method">方法</param>
		/// <param name="args">调用参数</param>
		public CallContext(MethodInfo method, object[] args) : this(null, method, args) {
		}

		/// <summary>
		/// 默认构造方法
		/// </summary>
		public CallContext() {
		}

		/// <summary>
		/// 被调用的对象实例
		/// </summary>
		public object ObjectInstance { get; set; }
		/// <summary>
		/// 方法
		/// </summary>
		public MethodInfo CallMethod { get; set; }
		/// <summary>
		/// 调用参数
		/// </summary>
		public object[] CallArguments { get; set; }
		/// <summary>
		/// 方法是否已被调用
		/// </summary>
		public bool ResultInitialized { get; set; }
		/// <summary>
		/// 方法返回值
		/// </summary>
		public object ResultValue { get; set; }
		/// <summary>
		/// 调用过程中可能产生的错误
		/// </summary>
		public Exception Error { get; set; }

		private IDictionary items;
		/// <summary>
		/// 其他参数
		/// </summary>
		public IDictionary Items {
			get => this.items ?? (this.items = new HybridDictionary());
			set => this.items = value;
		}
	}
}

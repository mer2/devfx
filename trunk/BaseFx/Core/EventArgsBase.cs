/* Copyright(c) 2005-2011 R2@DevFx.NET, License(LGPL) */

using System;

namespace HTB.DevFx.Core
{
	/// <summary>
	/// 事件参数基础类
	/// </summary>
	[Serializable]
	public class EventArgsBase : EventArgs
	{
		/// <summary>
		/// 构造方法
		/// </summary>
		/// <param name="sender">事件发生者</param>
		public EventArgsBase(object sender) : this(sender, null) {}

		/// <summary>
		/// 构造方法
		/// </summary>
		/// <param name="sender">事件发生者</param>
		/// <param name="eventType">事件类型</param>
		public EventArgsBase(object sender, string eventType) {
			this.Sender = sender;
			this.EventType = eventType;
		}

		/// <summary>
		/// 事件发生者
		/// </summary>
		public virtual object Sender { get; private set; }

		/// <summary>
		/// 事件类型
		/// </summary>
		public virtual string EventType { get; private set; }
	}

	/// <summary>
	/// 事件参数基础类（泛型）
	/// </summary>
	[Serializable]
	public class EventArgsBase<T> : EventArgsBase
	{
		/// <summary>
		/// 构造方法
		/// </summary>
		/// <param name="sender">事件发生者</param>
		/// <param name="eventType">事件类型</param>
		/// <param name="eventValue">事件附加值</param>
		public EventArgsBase(object sender, string eventType, T eventValue) : base(sender, eventType) {
			this.eventValue = eventValue;
		}

		/// <summary>
		/// 事件附加值
		/// </summary>
		public virtual T EventValue {
			get { return this.eventValue; }
			set { this.eventValue = value; }
		}

		private T eventValue;
	}

	/// <summary>
	/// 事件委托（泛型）
	/// </summary>
	/// <typeparam name="T">事件参数类型</typeparam>
	/// <param name="sender">事件发生者</param>
	/// <param name="e">事件参数</param>
	[Serializable]
	public delegate void EventHandlerDelegate<T>(object sender, T e) where T : EventArgsBase;

	/// <summary>
	/// 事件委托（泛型）
	/// </summary>
	/// <typeparam name="T">事件参数类型</typeparam>
	/// <param name="sender">事件发生者</param>
	/// <param name="e">事件参数</param>
	[Serializable]
	public delegate void EventHandlerDelegates<T>(object sender, T[] e) where T : EventArgsBase;

	/// <summary>
	/// 事件委托（泛型）
	/// </summary>
	/// <typeparam name="T">事件参数类型</typeparam>
	/// <typeparam name="V">事件附加值类型</typeparam>
	/// <param name="sender">事件发生者</param>
	/// <param name="e">事件参数</param>
	[Serializable]
	public delegate void EventHandlerDelegate<T, V>(object sender, T e) where T : EventArgsBase<V>;
}
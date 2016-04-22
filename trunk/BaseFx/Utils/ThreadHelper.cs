/* Copyright(c) 2005-2011 R2@DevFx.NET, License(LGPL) */

using System;

namespace HTB.DevFx.Utils
{
	/// <summary>
	/// 关于线程的一些实用方法
	/// </summary>
	internal static class ThreadHelper
	{
		/// <summary>
		/// 线程安全执行某段代码
		/// </summary>
		/// <param name="lockObject">安全控制对象</param>
		/// <param name="setter">条件，返回true表示需要执行线程安全代码</param>
		/// <param name="executor">需要线程安全的代码段</param>
		public static void ThreadSafeExecute(object lockObject, Func<bool> setter, Action executor) {
			if (lockObject == null || setter == null || executor == null) {
				return;
			}
			if (setter()) {
				lock (lockObject) {
					if (setter()) {
						executor();
					}
				}
			}
		}
	}
}

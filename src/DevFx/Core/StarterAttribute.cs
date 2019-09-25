using DevFx.Exceptions;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

namespace DevFx.Core
{
	[Serializable, AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
	public abstract class StarterAttribute : Attribute
	{
		public bool Enabled { get; set; } = true;
		protected virtual void OnStart(IDictionary items) {
		}

		public static bool IsEnabled<T>() where T : StarterAttribute {
			var current = GetCurrent<T>();
			return current != null ? current.Enabled : false;
		}
		public static T GetCurrent<T>() where T : StarterAttribute {
			StarterAttributes.TryGetValue(typeof(T), out var starter);
			return (T)starter;
		}
		private static readonly Dictionary<Type, StarterAttribute> StarterAttributes = new Dictionary<Type, StarterAttribute>();
		internal static void Init(IDictionary items) {
			var entryPoint = Assembly.GetEntryAssembly().EntryPoint;
			var starters = entryPoint.GetCustomAttributes<StarterAttribute>(true);
			foreach (var starter in starters) {
				StarterAttributes.Add(starter.GetType(), starter);
				try {
					starter.OnStart(items);
				} catch (Exception e) {
					ExceptionService.Publish(e);
				}
			}
		}
	}
}
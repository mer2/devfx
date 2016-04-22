/* Copyright(c) 2005-2011 R2@DevFx.NET, License(LGPL) */

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;

namespace HTB.DevFx.Data
{
	public static class DataExtensions
	{
		public static IObjectWrap Wrap(this object value) {
			return ObjectWrap.Wrap(value);
		}

		public static object Execute(this IDataOperation dataOperation, object parameters) {
			if(dataOperation == null) {
				throw new ArgumentNullException("dataOperation");
			}
			var statementName = GetMethodNameFromRuntimeStack(2);
			return dataOperation.Execute(statementName, parameters);
		}

		public static T Execute<T>(this IDataOperation dataOperation, object parameters) {
			if (dataOperation == null) {
				throw new ArgumentNullException("dataOperation");
			}
			var statementName = GetMethodNameFromRuntimeStack(2);
			return dataOperation.Execute<T>(statementName, parameters);
		}

		public static object Execute(this IDataOperation dataOperation, object parameters, object result) {
			if (dataOperation == null) {
				throw new ArgumentNullException("dataOperation");
			}
			var statementName = GetMethodNameFromRuntimeStack(2);
			return dataOperation.Execute(statementName, parameters, result);
		}

		public static T Execute<T>(this IDataOperation dataOperation, object parameters, T result) {
			if (dataOperation == null) {
				throw new ArgumentNullException("dataOperation");
			}
			var statementName = GetMethodNameFromRuntimeStack(2);
			return dataOperation.Execute(statementName, parameters, result);
		}

		internal static string GetMethodNameFromRuntimeStack(int skipFrames) {
			var st = new StackTrace(skipFrames, false);
			var fr = st.GetFrame(0);
			if(fr != null) {
				var method = fr.GetMethod();
				if(method != null) {
					return method.Name;
				}
			}
			return null;
		}

		internal static Dictionary<string, object> ToDictionary(this IDictionary dictionary, IEqualityComparer<string> comparer) {
			Dictionary<string, object> dict;
			if(dictionary is IDictionary<string, object>) {
				dict = new Dictionary<string, object>((IDictionary<string, object>)dictionary, comparer);
			} else {
				dict = new Dictionary<string, object>(comparer);
				foreach(var key in dictionary.Keys) {
					dict.Add(Convert.ToString(key), dictionary[key]);
				}
			}
			return dict;
		}
	}
}

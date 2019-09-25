using DevFx.Esb.Serialize;
using DevFx.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Net.Http;
using System.Reflection;

namespace DevFx.Esb.Client
{
	internal class HttpRealProxyPerformance : HttpRealProxy
	{
		public HttpRealProxyPerformance(Type proxyType, string url, string contentType) : base(proxyType, url, contentType) {}

		protected override object Invoke(MethodInfo targetMethod, object[] args) {
			return ElapseTime("Invoke", () => base.Invoke(targetMethod, args));
		}

		protected override object Call(IDictionary<string, object> parameters, MethodInfo methodMessage) {
			return ElapseTime("Call", () => base.Call(parameters, methodMessage));
		}

		protected override void PrepareRequest(ProxyContext ctx) {
			ElapseTime("PrepareRequest", () => base.PrepareRequest(ctx));
		}

		protected override object ResponseHandle(ProxyContext ctx, HttpResponseMessage response) {
			return ElapseTime("ResponseHandle`2", () => base.ResponseHandle(ctx, response));
		}

		protected override object ResultHandle(IAOPResult aop, Type returnType, ISerializer serializer) {
			return ElapseTime("ResponseHandle`3", () => base.ResultHandle(aop, returnType, serializer));
		}

		private static T ElapseTime<T>(string message, Func<T> func) {
			var stopwatcher = new Stopwatch();
			stopwatcher.Start();
			var result = func();
			stopwatcher.Stop();
			LogService.Debug($"Invoke {message}:{stopwatcher.ElapsedMilliseconds}", nameof(HttpRealProxyPerformance));
			return result;
		}

		private static void ElapseTime(string message, Action func) {
			var stopwatcher = new Stopwatch();
			stopwatcher.Start();
			func();
			stopwatcher.Stop();
			LogService.Debug($"Invoke {message}:{stopwatcher.ElapsedMilliseconds}", nameof(HttpRealProxyPerformance));
		}
	}
}

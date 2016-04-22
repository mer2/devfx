/* Copyright(c) 2005-2011 R2@DevFx.NET, License(LGPL) */

using System;
using HTB.DevFx.Log;
using HTB.DevFx.Exceptions;

namespace HTB.DevFx.Remoting
{
	public abstract class RemotingClientProxy<TObject> where TObject : class
	{
		protected RemotingClientProxy(string url) {
			this.RemotingUri = url;
		}
		public string RemotingUri { get; protected set; }

		private TObject remotingObject;
		protected virtual TObject RemotingObject {
			get { return this.remotingObject ?? (this.remotingObject = (TObject)Activator.GetObject(typeof(TObject), this.RemotingUri)); }
		}

		protected virtual bool RemotingIsReady() {
			var objectInstance = this.RemotingObject;
			if (objectInstance == null) {
				return false;
			}
			try {
				objectInstance.ToString();
				return true;
			} catch (Exception e) {
				ExceptionService.Publish(new RemotingException("远程服务器不可用，请检查！", e), LogLevel.EMERGENCY);
				return false;
			}
		}
	}
}

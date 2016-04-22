/* Copyright(c) 2005-2011 R2@DevFx.NET, License(LGPL) */

using System;
using HTB.DevFx.Remoting;
using HTB.DevFx.Utils;

namespace HTB.DevFx.Services
{
	public abstract class RemotingRunner : MarshalByRefObject, IServiceRunner
	{
		private string remotingConfigFile;
		protected string RemotingConfigFile {
			get { return this.remotingConfigFile; }
			set {
				this.remotingConfigFile
					= string.IsNullOrEmpty(value)
						? AppDomain.CurrentDomain.SetupInformation.ConfigurationFile
						: FileHelper.GetFullPath(value);
			}
		}

		protected RemotingRunner(string configFile) {
			this.RemotingConfigFile = configFile;
		}

		protected virtual void RegisterRemotingObject(string configFile) {
			RemotingHelper.RegisterRemotingObject(configFile);
		}

		protected virtual void StartInternal() {
			this.RegisterRemotingObject(this.RemotingConfigFile);
		}

		protected virtual void StopInternal() {}

		#region IServiceRunner Members

		void IServiceRunner.Start() {
			this.StartInternal();
		}

		void IServiceRunner.Stop() {
			this.StopInternal();
		}

		#endregion
	}
}
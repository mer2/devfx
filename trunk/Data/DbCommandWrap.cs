/* Copyright(c) 2005-2011 R2@DevFx.NET, License(LGPL) */

using System;
using System.Data.Common;
using HTB.DevFx.Log;

namespace HTB.DevFx.Data
{
	internal class DbCommandWrap : IDbCommandWrap
	{
		public DbCommandWrap(DbCommand command, bool closeOnDisposed) {
			this.Command = command;
			this.closeOnDisposed = closeOnDisposed;
		}

		public DbCommand Command { get; private set; }

		private readonly bool closeOnDisposed;

		#region IDisposable Members

		protected void Dispose(bool disposing) {
			if(disposing) {
				if(this.closeOnDisposed) {
					if(this.Command != null && this.Command.Connection != null) {
						var connection = this.Command.Connection;
						connection.Close();
						this.Command.Dispose();
						var dataService = DataService.Current;
						if(dataService is DataServiceBase && ((DataServiceBase)dataService).Debug) {
							LogService.WriteLog(this, LogLevel.DEBUG, "Connection closed in command dispose:" + connection.GetHashCode());
						}
					}
					this.Command = null;
				}
			}
		}

		public void Dispose() {
			this.Dispose(true);
			GC.SuppressFinalize(this);
		}

		~DbCommandWrap() {
			this.Dispose(true);
		}

		#endregion
	}
}

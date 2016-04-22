/* Copyright(c) 2005-2011 R2@DevFx.NET, License(LGPL) */

using System;

namespace HTB.DevFx.Data
{
	partial class DataSessionBase : IDataSession
	{
		#region IDataSession Members

		void IDataSession.CommitTransaction() {
			this.CommitTransaction();
		}

		void IDataSession.RollbackTransaction() {
			this.RollbackTransaction();
		}

		#endregion

		#region IDisposable Members

		void IDisposable.Dispose() {
			this.Dispose(true);
			GC.SuppressFinalize(this);
		}

		~DataSessionBase() {
			this.Dispose(true);
		}

		#endregion
	}
}

/* Copyright(c) 2005-2011 R2@DevFx.NET, License(LGPL) */

using System;

namespace HTB.DevFx.Pooling
{
	public abstract class PoolObjectBase : IPoolable, IDisposable
	{
		protected event Action<IPoolable, bool> Disposing;
		protected virtual void Dispose(bool disposing) {
		}
		protected virtual bool IsAlive { get { return true; } }
		protected virtual void Free() {
			if (this.Disposing != null) {
				this.Disposing(this, this.IsAlive);
			}
		}

		event Action<IPoolable, bool> IPoolable.Disposing {
			add { this.Disposing += value; }
			remove { this.Disposing -= value; }
		}


		void IPoolable.Dispose() {
			this.Dispose(true);
		}

		void IDisposable.Dispose() {
			this.Free();
		}
	}
}

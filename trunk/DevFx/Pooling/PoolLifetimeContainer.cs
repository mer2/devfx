/* Copyright(c) 2005-2011 R2@DevFx.NET, License(LGPL) */

using System;
using System.Collections;
using System.Collections.Generic;
using HTB.DevFx.Config;
using HTB.DevFx.Core;
using HTB.DevFx.Core.Config;
using HTB.DevFx.Core.Lifetime;
using HTB.DevFx.Exceptions;
using HTB.DevFx.Log;
using HTB.DevFx.Pooling.Config;

namespace HTB.DevFx.Pooling
{
	public class PoolLifetimeContainer : SingletonLifetimeContainer
	{
		protected override void Init(IObjectSetting objectSetting, IObjectBuilder objectBuilder) {
			base.Init(objectSetting, objectBuilder);
			var setting = objectSetting.ConfigSetting;
			var poolSetting = (IPoolLifetimeContainerSetting)setting.ToCachedSetting<PoolLifetimeContainerSetting>("pooling");
			if (poolSetting != null) {
				this.Enabled = poolSetting.Enabled;
				this.Debug = poolSetting.Debug;
				this.MaxPoolSize = poolSetting.MaxPoolSize;
			}
			var maxPoolSize = this.MaxPoolSize;
			if(maxPoolSize == 0 || !this.Enabled) {
				this.MaxPoolSize = maxPoolSize;
				this.Enabled = false;
				return;
			}
			if(maxPoolSize < 0) {
				maxPoolSize = int.MaxValue;
				this.Free = new Stack();
				this.Busy = new HashSet<object>();
			} else {
				this.Free = new Stack(maxPoolSize);
				this.Busy = new HashSet<object>();
			}
			this.MaxPoolSize = maxPoolSize;
		}

		protected override object GetObjectInternal(IDictionary items) {
			if(!this.Enabled) {
				return base.GetObjectInternal(items);
			}
			object result = null;
			lock(this.Free) {
				if(this.Free.Count > 0) {
					result = this.Free.Pop();
				}
			}
			if (result == null) {
				lock(this.Busy) {
					if (this.Busy.Count < this.MaxPoolSize) {
						result = this.ObjectBuilder.CreateObject(this.ObjectSetting, items);
						if(!(result is IPoolable)) {
							throw new Exception(string.Format("Object is not poolable.({0})", result.GetType().FullName));
						}
						var pool = (IPoolable)result;
						pool.Disposing += this.ObjectDisposing;
						if(this.Debug) {
							LogService.WriteLog(this, "Object Created {0}: Pooled {1}, Busy {2}", result.GetType().FullName, this.Free.Count, this.Busy.Count);
						}
					}
				}
			}
			if (result == null) {
				throw new Exception(string.Format("Max Pool Size {0} exceed.", this.MaxPoolSize));
			}
			lock (this.Busy) {
				this.Busy.Add(result);
			}
			return result;
		}

		private void ObjectDisposing(IPoolable pooledObject, bool alive) {
			try {
				lock(this.Busy) {
					this.Busy.Remove(pooledObject);
				}
				if(alive) {
					lock(this.Free) {
						if(!this.Free.Contains(pooledObject)) {
							this.Free.Push(pooledObject);
						}
					}
				} else {
					pooledObject.Dispose();
				}
			} catch(Exception e) {
				ExceptionService.Publish(e);
			}
		}

		private bool Enabled = true;
		private bool Debug;
		private int MaxPoolSize = -1;
		private Stack Free;
		private HashSet<object> Busy;
	}
}

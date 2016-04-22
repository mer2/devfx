/* Copyright(c) 2005-2011 R2@DevFx.NET, License(LGPL) */

using System;
using System.Collections;
using HTB.DevFx.Core;

namespace HTB.DevFx.Data
{
	internal class DbResultContext : ObjectContextBase, IDbResultContext
	{
		public DbResultContext(IDbExecuteContext ctx, Type resultType, object resultInstance, IResultHandler resultHandler, IDictionary items) : base(items) {
			this.ExecuteContext = ctx;
			this.ResultType = resultType;
			this.ResultInstance = resultInstance;
			this.ResultHandler = resultHandler;
		}
		
		public IDbExecuteContext ExecuteContext { get; private set; }
		public Type ResultType { get; set; }
		public object ResultInstance { get; set; }
		public IResultHandler ResultHandler { get; set; }
		public bool ResultHandled { get; set; }
	}
}

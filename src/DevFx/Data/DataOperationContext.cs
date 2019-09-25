/* Copyright(c) 2005-2011 R2@DevFx.NET, License(LGPL) */

using System.Collections;
using DevFx.Core;

namespace DevFx.Data
{
	public class DataOperationContext : ObjectContextBase, IDataOperationContext
	{
		public DataOperationContext(IDataOperation dataOperation, IDictionary items) : base(items) {
			this.DataOperation = dataOperation;
		}

		public IDataOperation DataOperation {
			get { return this.GetItem<IDataOperation>(); }
			set { this.SetItem(value); }
		}
	}
}

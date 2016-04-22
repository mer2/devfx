/* Copyright(c) 2005-2011 R2@DevFx.NET, License(LGPL) */

using System.Collections;
using System.Collections.Specialized;

namespace HTB.DevFx.Core
{
	internal class ObjectServiceContext : ObjectContextBase, IObjectServiceContext
	{
		public ObjectServiceContext(IObjectService objectService) : this(objectService, new HybridDictionary()) {
		}

		public ObjectServiceContext(IObjectService objectService, IDictionary items) : base(items) {
			this.ObjectService = objectService;
		}

		/// <summary>
		/// 当前<see cref="IObjectService"/>的实例
		/// </summary>
		public IObjectService ObjectService {
			get { return this.GetItem<IObjectService>(); }
			private set { this.SetItem(value); }
		}
	}
}

/* Copyright(c) 2005-2011 R2@DevFx.NET, License(LGPL) */

using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using DevFx.Configuration;

namespace DevFx.Core
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
		public IObjectService ObjectService { get; }
		public IObjectNamespace GlobalObjectNamespace { get; set; }
		public IDictionary<Type, IList<CoreAttribute>> CoreAttributes { get; set; }
		public IConfigSetting ConfigSetting { get; set; }
	}
}

/* Copyright(c) 2005-2011 R2@DevFx.NET, License(LGPL) */

using DevFx.Configuration;
using System;
using System.Collections.Generic;

namespace DevFx.Core
{
	public interface IObjectServiceContext : IObjectContext
	{
		IObjectService ObjectService { get; }
		IObjectNamespace GlobalObjectNamespace { get; }
		IDictionary<Type, IList<CoreAttribute>> CoreAttributes { get; }
		IConfigSetting ConfigSetting { get; }
	}
}

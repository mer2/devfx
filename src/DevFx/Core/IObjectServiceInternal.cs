/* Copyright(c) 2005-2011 R2@DevFx.NET, License(LGPL) */

using DevFx.Configuration;
using System;
using System.Collections.Generic;

namespace DevFx.Core
{
	public interface IObjectServiceInternal
	{
		void OnObjectCreating(IObjectBuilderContext ctx);//提供给IObjectBuilder调用
		void OnObjectCreated(IObjectBuilderContext ctx);//提供给IObjectBuilder调用

		IDictionary<Type, IList<CoreAttribute>> CoreAttributes { get; }
		IConfigSetting ConfigSetting { get; }
	}
}
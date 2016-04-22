/* Copyright(c) 2005-2011 R2@DevFx.NET, License(LGPL) */

using System;
using System.Collections;
using HTB.DevFx.Core.Config;

namespace HTB.DevFx.Core
{
	public interface IObjectBuilder
	{
		object CreateObject(IObjectSetting objectSetting);
		object CreateObject(IObjectSetting objectSetting, params object[] args);

		object CreateObject(IObjectSetting objectSetting, IDictionary items);
		object CreateObject(IObjectSetting objectSetting, IDictionary items, params object[] args);

		event Action<IObjectBuilderContext> ObjectCreating;
		event Action<IObjectBuilderContext> ObjectCreated;
	}
}

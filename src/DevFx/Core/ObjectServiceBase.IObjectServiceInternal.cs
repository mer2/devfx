/* Copyright(c) 2005-2011 R2@DevFx.NET, License(LGPL) */

using DevFx.Configuration;
using System;
using System.Collections.Generic;

namespace DevFx.Core
{
	partial class ObjectServiceBase : IObjectServiceInternal
	{
		#region IObjectServiceInternal Members

		void IObjectServiceInternal.OnObjectCreating(IObjectBuilderContext ctx) => this.OnObjectCreating(ctx);
		void IObjectServiceInternal.OnObjectCreated(IObjectBuilderContext ctx) => this.OnObjectCreated(ctx);
		IConfigSetting IObjectServiceInternal.ConfigSetting => this.ConfigSetting;
		IDictionary<Type, IList<CoreAttribute>> IObjectServiceInternal.CoreAttributes => this.CoreAttributes;

		#endregion
	}
}
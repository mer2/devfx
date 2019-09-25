/* Copyright(c) 2005-2011 R2@DevFx.NET, License(LGPL) */

using System.Collections;

namespace DevFx.Core
{
	public interface ILifetimePolicy : IObjectContext
	{
		object GetObject(IDictionary items);
		void SetObject(object newValue, IDictionary items);
		void RemoveObject(IDictionary items);
	}
}

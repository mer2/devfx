/* Copyright(c) 2005-2011 R2@DevFx.NET, License(LGPL) */

using DevFx.Core;

namespace DevFx.Data
{
	public interface IDataOperationContext : IObjectContext
	{
		IDataOperation DataOperation { get; }
	}
}

/* Copyright(c) 2005-2011 R2@DevFx.NET, License(LGPL) */

using HTB.DevFx.Core;

namespace HTB.DevFx.Data
{
	public interface IDataOperationContext : IObjectContext
	{
		IDataOperation DataOperation { get; }
	}
}

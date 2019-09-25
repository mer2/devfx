/* Copyright(c) 2005-2013 R2@DevFx.NET, License(LGPL) */

using System.Data.Common;

namespace DevFx.Data
{
	/// <summary>
	/// 创建<see cref="DbCommand" />
	/// </summary>
	[Service]
	public interface IDbCommandBuilder
	{
		DbCommand GetCommand(IDbExecuteContext ctx);
	}
}
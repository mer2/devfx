/* Copyright(c) 2005-2011 R2@DevFx.NET, License(LGPL) */

namespace HTB.DevFx.Exceptions.Config
{
	public interface IExceptionServiceSetting
	{
		bool Enabled { get; }
		IExceptionHandlerSetting[] ExceptionHandlers { get; }
	}
}

/* Copyright(c) 2005-2011 R2@DevFx.NET, License(LGPL) */

using System;

namespace HTB.DevFx.Exceptions
{
	public class ExceptionFormatter : IExceptionFormatter
	{
		#region IExceptionFormatter Members

		public virtual string GetFormatString(Exception e, object attachObject) {
			return e.ToString();
		}

		#endregion
	}
}

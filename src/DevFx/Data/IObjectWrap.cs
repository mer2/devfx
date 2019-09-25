/* Copyright(c) 2005-2011 R2@DevFx.NET, License(LGPL) */

namespace DevFx.Data
{
	public interface IObjectWrap
	{
		object Value { get; }
		void ChangeValue(object value);
	}
}

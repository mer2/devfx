/* Copyright(c) 2005-2011 R2@DevFx.NET, License(LGPL) */

namespace HTB.DevFx.Data.Config
{
	public interface IPropertyMapSetting
	{
		string PropertyName { get; }
		string ColumnName { get; }
		object DefaultValue { get; }
	}
}

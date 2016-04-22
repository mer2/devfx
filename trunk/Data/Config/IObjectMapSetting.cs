/* Copyright(c) 2005-2011 R2@DevFx.NET, License(LGPL) */

namespace HTB.DevFx.Data.Config
{
	public interface IObjectMapSetting
	{
		string IncludeProperties { get; }
		string ExcludeProperties { get; }
		string TypeTranslatorName { get; }
		bool IgnoreCase { get; }
		IPropertyMapSetting[] Properties { get; }
	}
}

/* Copyright(c) 2005-2011 R2@DevFx.NET, License(LGPL) */

using HTB.DevFx.Config;

namespace HTB.DevFx.Data.Config
{
	internal interface IDynamicTextSetting : IConfigSettingRequired
	{
		string ParameterName { get; }
		bool? IsNull { get; }
		bool? IsPresent { get; }

		IDynamicTextSetting[] DynamicTexts { get; }
	}
}

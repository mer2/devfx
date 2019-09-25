/* Copyright(c) 2005-2011 R2@DevFx.NET, License(LGPL) */

using System;
using System.Data;

namespace DevFx.Data.Settings
{
	public interface IParameterSetting
	{
		string Name { get; }
		string ParameterName { get; }
		string ParameterTypeName { get; }
		bool IsInline { get; }
		bool Expandable { get; }//是否可扩展
		string DbTypeName { get; }
		ParameterDirection Direction { get; }
		bool IsNullable { get; }
		int Size { get; }
		byte Scale { get; }
		byte Precision { get; }
		object GetDefaultValue(Type type);
	}
}

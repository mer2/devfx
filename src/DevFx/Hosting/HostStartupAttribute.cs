using DevFx.Core;
using System;

namespace DevFx.Hosting
{
	[Serializable, AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
	public class HostStartupAttribute : CoreAttribute
	{
	}
}

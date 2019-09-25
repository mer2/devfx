using DevFx.Core;
using System;

namespace DevFx.Web
{
	[Serializable, AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
	public class MiddlewareAttribute : CoreAttribute
	{
	}
}

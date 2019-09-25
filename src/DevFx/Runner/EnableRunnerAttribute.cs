using DevFx.Core;
using System;

namespace DevFx.Runner
{
	[Serializable, AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
	public class EnableRunnerAttribute : StarterAttribute
	{
	}
}
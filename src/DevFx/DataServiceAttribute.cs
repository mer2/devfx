using System;

namespace DevFx
{
	[Serializable, AttributeUsage(AttributeTargets.Interface, AllowMultiple = false)]
	public class DataServiceAttribute : ServiceAttribute
	{
		public string GroupName { get; set; }
	}
}
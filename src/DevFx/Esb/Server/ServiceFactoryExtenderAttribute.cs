using DevFx.Core;
using System;

namespace DevFx.Esb.Server
{
	[Serializable, AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
	public class ServiceFactoryExtenderAttribute : ServiceExtenderAttribute
	{
	}
}

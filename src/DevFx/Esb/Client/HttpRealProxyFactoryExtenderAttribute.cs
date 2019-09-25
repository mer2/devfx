using DevFx.Core;
using System;

namespace DevFx.Esb.Client
{
	[Serializable, AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
	public class HttpRealProxyFactoryExtenderAttribute : ServiceExtenderAttribute
	{
	}
}
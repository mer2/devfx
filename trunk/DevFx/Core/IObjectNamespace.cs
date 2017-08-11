using System.Collections.Generic;

namespace HTB.DevFx.Core
{
	public interface IObjectNamespace
	{
		string Name { get; }
		IDictionary<string, string> TypeAliases { get; }
		IDictionary<string, object> ConstAliases { get; }
		IDictionary<string, ILifetimeContainer> ObjectAliases { get; }
	}
}
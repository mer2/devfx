using System.Collections.Generic;

namespace DevFx.Core
{
	public interface ICoreAttributeHandler<T> where T : CoreAttribute
	{
		void HandleAttributes(IObjectServiceContext ctx, IList<CoreAttribute> attributes);
	}
}
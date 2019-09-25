using System.Reflection;

namespace DevFx.Reflection
{
	public interface IServiceDispatchProxy
	{
		object Invoke(MethodInfo targetMethod, object[] args);
	}
}
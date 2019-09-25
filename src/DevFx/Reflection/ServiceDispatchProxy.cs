using System.Reflection;

namespace DevFx.Reflection
{
	public class ServiceDispatchProxy : DispatchProxy
	{
		public virtual IServiceDispatchProxy Wrapper { get; set; }
		protected override object Invoke(MethodInfo targetMethod, object[] args) {
			return this.Wrapper?.Invoke(targetMethod, args);
		}
	}

	public class ServiceDispatchProxy<T> : DispatchProxy where T : IServiceDispatchProxy
	{
		public virtual T Wrapper { get; set; }
		protected override object Invoke(MethodInfo targetMethod, object[] args) {
			return this.Wrapper?.Invoke(targetMethod, args);
		}
	}
}
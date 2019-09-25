using System;

namespace DevFx.Esb.Server
{
	[AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
	public class MethodNameAttribute : Attribute
	{
		public MethodNameAttribute(string name) {
            if (string.IsNullOrEmpty(name)) {
                throw new ArgumentNullException(nameof(name));
            }
            this.Name = name;
        }
 
        public string Name { get; }
	}
}

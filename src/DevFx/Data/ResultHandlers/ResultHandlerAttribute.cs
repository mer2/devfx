using DevFx.Core;
using System;

namespace DevFx.Data.ResultHandlers
{
	[Serializable, AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
	public class ResultHandlerAttribute : CoreAttribute
	{
		public ResultHandlerAttribute(Type type) {
			this.HandleType = type;
		}

		public Type HandleType { get; set; }
	}
}

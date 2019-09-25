using System;
using DevFx.Core;

namespace DevFx
{
	[Serializable, AttributeUsage(AttributeTargets.Interface, AllowMultiple = false)]
	public class RemotingServiceAttribute : CoreAttribute
	{
		public RemotingServiceAttribute(string url) {
			if(string.IsNullOrEmpty(url)) {
				throw new ArgumentNullException(nameof(url));
			}
			this.Url = url;
		}

		public string Url { get; set; }
		public string ContentType { get; set; }
	}
}
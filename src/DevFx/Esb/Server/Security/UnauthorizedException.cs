using System;

namespace DevFx.Esb.Server.Security
{
	[Serializable]
	public class UnauthorizedException : Exception
	{
		public UnauthorizedException() {}

		public UnauthorizedException(string message) : base(message) { }
	}
}
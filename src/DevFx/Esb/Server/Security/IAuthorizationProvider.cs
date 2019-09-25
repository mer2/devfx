namespace DevFx.Esb.Server.Security
{
	public interface IAuthorizationProvider
	{
		bool Authorize(ServiceContext ctx, IAuthorizationIdentity identity);
	}
}
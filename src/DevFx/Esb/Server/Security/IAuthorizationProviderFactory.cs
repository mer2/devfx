namespace DevFx.Esb.Server.Security
{
	[Service]
	internal interface IAuthorizationProviderFactory
	{
		void Authorize(ServiceContext ctx, IAuthorizationProviderFactory defaultFactory, IAuthorizationIdentity[] identities);
	}
}
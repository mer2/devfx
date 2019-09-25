namespace DevFx.Esb.Server.Security
{
	public interface IAuthorizationIdentity
	{
		string Users { get; }
		string Roles { get; }
		string Category { get; }
	}
}
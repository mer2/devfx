using System.ServiceModel;

namespace ClassLibrary2
{
	[ServiceContract]
	public interface IClassService
	{
		[OperationContract]
		string HelloWorld();
	}
}

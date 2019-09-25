using Microsoft.Extensions.DependencyInjection;
using System;

namespace DevFx.Hosting
{
	public interface IHostBuilderInternal
	{
		object GetBuilderWrapper();
		void ConfigureServices(Action<IServiceCollection> configureServices);
	}
}
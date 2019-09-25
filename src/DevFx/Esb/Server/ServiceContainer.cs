using System;

namespace DevFx.Esb.Server
{
	internal class ServiceContainer : IServiceContainer
	{
		/// <summary>
		/// 服务发布名称，形如：http://domain/Services/{service}
		/// </summary>
		public string Name { get; set; }
		/// <summary>
		/// 服务统一名称，形如：esb://octopus.xxxservice
		/// </summary>
		public string AliasName { get; set; }
		public string Authorization { get; set; }

		internal bool Inherits { get; set; }
		internal Type ContractType { get; set; }
		internal Type ServiceType { get; set; }
		internal ServiceHandler ServiceHandler { get; set; }
	}
}
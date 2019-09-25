using System;
using System.Collections;
using System.Collections.Specialized;

namespace DevFx.Esb.Client
{
	public class ProxyFactoryContext
	{
		public IHttpRealProxyFactory ProxyFactory { get; set; }
		public IHttpRealProxy ProxyInstance { get; set; }
		public Type ContractType { get; set; }
		public string ProxyUrl { get; set; }
		public string ContentType { get; set; }

		private IDictionary items;
		/// <summary>
		/// 其他参数
		/// </summary>
		public IDictionary Items {
			get => this.items ?? (this.items = new HybridDictionary());
			set => this.items = value;
		}
	}
}

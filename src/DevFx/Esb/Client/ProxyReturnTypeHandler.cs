using DevFx.Core;
using System;
using System.Collections;
using System.Collections.Generic;

namespace DevFx.Esb.Client
{
	//远程对象返回值重新定义
	[Object, HttpRealProxyFactoryExtender]
	internal class ProxyReturnTypeHandler : IObjectExtender<IHttpRealProxyFactory>
	{
		[Autowired]
		protected IObjectService ObjectService { get; set; }
		private readonly IDictionary<string, Type> returnTypes = new Dictionary<string, Type>();
		public void Init(IHttpRealProxyFactory instance, IDictionary items) {
			var coreAttributes = this.ObjectService.AsObjectServiceInternal().CoreAttributes;
			coreAttributes.TryGetValue(typeof(ProxyReturnTypeAttribute), out var list);
			if(list != null && list.Count > 0) {
				foreach(ProxyReturnTypeAttribute attribute in list) {
					var type = attribute.OwnerType;
					if(type != null) {
						this.returnTypes.Add(attribute.Name, type);
					}
				}
			}
			instance.Request += this.OnRequest;
		}

		private void OnRequest(ProxyContext ctx) {
			if (ctx.ExpectedReturnType != null) {
				return;
			}
			var name = $"{ctx.ProxyInstance.ProxyType}.{ctx.CallMethod.Name}";
			if(this.returnTypes.TryGetValue(name, out var type)) {
				ctx.ExpectedReturnType = type;
			}
		}
	}
}
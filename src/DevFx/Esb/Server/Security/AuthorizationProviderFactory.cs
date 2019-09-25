using DevFx.Core;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace DevFx.Esb.Server.Security
{
	[Object, ServiceFactoryExtender]
	internal class AuthorizationProviderFactory : IInitializable, IObjectExtender<IServiceFactory>, IAuthorizationProviderFactory
	{
		[Autowired]
		protected IObjectService ObjectService { get; set; }
		private Dictionary<string, IAuthorizationProvider> providers;

		void IInitializable.Init() {
			this.providers = new Dictionary<string, IAuthorizationProvider>();

			var coreAttributes = this.ObjectService.AsObjectServiceInternal().CoreAttributes;
			if (!coreAttributes.TryGetValue(typeof(AuthorizationProviderAttribute), out var list)) {
				return;
			}
			if(list == null || list.Count <= 0) {
				return;
			}
			foreach(AuthorizationProviderAttribute attribute in list) {
				var instance = this.ObjectService.GetObject(attribute.OwnerType);
				if(instance is IAuthorizationProvider) {
					this.providers.Add(attribute.Category, (IAuthorizationProvider)instance);
				}
			}
		}

		void IObjectExtender<IServiceFactory>.Init(IServiceFactory instance, IDictionary items) {
			instance.Calling += this.OnCalling;
		}

		public void Authorize(ServiceContext ctx, IAuthorizationProviderFactory defaultFactory, IAuthorizationIdentity[] identities) {
			if (identities == null) {
				return;
			}
			//判断是否有不需要验证的标记
			if(identities.Any(x => x.Category == NoneAuthorizeAttribute.AuthorizeCategory && x.Users == "*")) {
				return;
			}
			foreach(var identity in identities) {
				var category = identity.Category;
				if(string.IsNullOrEmpty(category) || !this.providers.TryGetValue(category, out var provider)) {
					continue;
				}
				if(!provider.Authorize(ctx, identity)) {
					if(!ctx.ResultInitialized) {
						ctx.ResultInitialized = true;
						ctx.ResultValue = AOPResult.Failed(-1, $"Authorized Failed with {category}");
					}
					break;
				}
			}
		}

		private void OnCalling(ServiceContext ctx) {
			var identities = ActionFilterAttribute.GetTypedFilters<IAuthorizationIdentity>(ctx);
			this.Authorize(ctx, this, identities);
		}
	}
}
using DevFx.Core;
using DevFx.Core.Lifetime;
using System;
using System.Collections;
using System.Reflection;

namespace DevFx.Esb.Client
{
	[ObjectServiceExtender]
	internal class RemotingObjectServiceExtender : IObjectExtender<IObjectService>
	{
		public void Init(IObjectService instance, IDictionary items) {
			this.ObjectBuilder = new RemotingServiceBuilder(instance);
			instance.ObjectContainerGetted += this.OnObjectContainerGetted;
		}
		private RemotingServiceBuilder ObjectBuilder { get; set; }

		private void OnObjectContainerGetted(IObjectContainerContext ctx) {
			if (ctx.Container != null) {
				return;
			}
			var objectKey = ctx.ObjectKey;
			if (!(objectKey is Type)) {
				return;
			}
			var objectType = (Type)objectKey;
			if (!objectType.IsDefined(typeof(RemotingServiceAttribute), true)) {
				return;
			}
			var attribute = objectType.GetCustomAttribute<RemotingServiceAttribute>();
			//自动生成
			IObjectContainer container = new SingletonObjectContainer();
			container.Init(new ObjectDescription(objectType, new Hashtable { { typeof(RemotingServiceAttribute), attribute } }), this.ObjectBuilder);
			ctx.Namespace.AddObject(objectType, container);
			ctx.Container = container;
		}

		internal class RemotingServiceBuilder : ObjectBuilderBase
		{
			public RemotingServiceBuilder(IObjectService objectService) : base(objectService) {
			}

			protected override void CreateObjectInternal(IObjectBuilderContext ctx, params object[] args) {
				var attribute = ctx.ObjectDescription.Items[typeof(RemotingServiceAttribute)] as RemotingServiceAttribute;
				var objectType = ctx.ObjectDescription.ObjectType;
				var uri = attribute.Url;
				var contentType = attribute.ContentType;
				var factory = this.ObjectService.GetObject<IHttpRealProxyFactory>();
				var instance = factory.GetProxyObject(objectType, uri, contentType);
				ctx.ObjectInstance = instance;
			}
		}
	}
}

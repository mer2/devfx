/* Copyright(c) 2005-2011 R2@DevFx.NET, License(LGPL) */

using HTB.DevFx.Esb.Config;

namespace HTB.DevFx.Esb
{
	internal class ObjectBuilderAdapter : IObjectBuilder
	{
		private readonly Core.IObjectBuilder coreBuilder;
		public ObjectBuilderAdapter(Core.IObjectBuilder coreBuilder) {
			this.coreBuilder = coreBuilder;
		}

		#region Implementation of IObjectBuilder

		public object CreateObject(IObjectSetting setting, params object[] parameters) {
			return this.coreBuilder.CreateObject(setting as Core.Config.IObjectSetting, parameters);
		}

		#endregion
	}

	internal class ServiceLocatorAdapter : ServiceLocator.ServiceLocatorBase
	{
		private ObjectBuilderAdapter builderAdapter;
		public override void Init(IServiceLocatorSetting setting) {
			var builder = ObjectService.Current.ObjectBuilder;
			this.builderAdapter = new ObjectBuilderAdapter(builder);
			builder.ObjectCreating += this.ObjectBuilderOnObjectCreating;
			builder.ObjectCreated += this.ObjectBuilderOnObjectCreated;
			base.Init(setting);
		}

		void ObjectBuilderOnObjectCreating(Core.IObjectBuilderContext ctx) {
			this.OnObjectBuilding(new ObjectBuildContext(this, ctx.ObjectSetting, this.builderAdapter, ctx.Items));
		}

		void ObjectBuilderOnObjectCreated(Core.IObjectBuilderContext ctx) {
			this.OnObjectBuilt(new ObjectBuildContext(this, ctx.ObjectSetting, this.builderAdapter, ctx.Items));
		}

		#region Overrides of ServiceLocator

		internal override T GetServiceInternal<T>() {
			return ObjectService.GetObject<T>();
		}

		internal override T GetServiceInternal<T>(string serviceName) {
			return ObjectService.GetObject<T>(serviceName);
		}

		internal override object GetServiceInternal(string serviceName) {
			return ObjectService.GetObject(serviceName);
		}

		internal override string GetTypeNameInternal(string typeAlias) {
			return ObjectService.GetTypeName(typeAlias);
		}

		#endregion
	}
}

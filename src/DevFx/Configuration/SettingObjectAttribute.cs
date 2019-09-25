using DevFx.Core;
using DevFx.Core.Lifetime;
using DevFx.Utils;
using System;
using System.Collections;
using System.Collections.Generic;

namespace DevFx.Configuration
{
	[Serializable, AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface, AllowMultiple = false)]
	public class SettingObjectAttribute : CoreAttribute
	{
		public SettingObjectAttribute() {}
		public SettingObjectAttribute(string xpath) {
			this.XPath = xpath;
		}

		public string XPath { get; set; }
		public bool Required { get; set; }
	}

	[CoreAttributeHandler]
	internal class SettingObjectAttributeHandler : ICoreAttributeHandler<SettingObjectAttribute>
	{
		public void HandleAttributes(IObjectServiceContext ctx, IList<CoreAttribute> attributes) {
			if(attributes == null || attributes.Count <= 0) {
				return;
			}
			var builder = new SettingObjectBuilder(ctx.ObjectService);
			foreach (SettingObjectAttribute attribute in attributes) {
				var items = new Hashtable { { typeof(SettingObjectAttribute), attribute } };
				var type = attribute.OwnerType;
				IObjectContainer container = new SingletonObjectContainer();
				container.Init(new ObjectDescription(type, items), builder);
				var objects = ctx.GlobalObjectNamespace;
				objects.AddObject(type, container);

				if(type.IsClass) {
					//查找此类型实现的接口
					var interfaces = type.GetInterfaces();
					if (interfaces.Length > 0) {
						foreach (var ifType in interfaces) {
							if (ifType.IsDefined(typeof(ServiceAttribute), true)) {
								objects.AddObject(ifType, container);
							}
						}
					}
				}
			}
		}
	}

	internal class SettingObjectBuilder : ObjectBuilderBase
	{
		public SettingObjectBuilder(IObjectService objectService) : base(objectService) {
		}

		protected override void CreateObjectInternal(IObjectBuilderContext ctx, params object[] args) {
			if (!(ctx.ObjectDescription.Items[typeof(SettingObjectAttribute)] is SettingObjectAttribute attribute)) {
				return;
			}
			var xpath = attribute.XPath;
			var type = attribute.OwnerType;
			if (string.IsNullOrEmpty(xpath)) {
				xpath = type.FullName.ToLowerInvariant();
			}
			var objectService = (IObjectServiceInternal)ctx.ObjectService;
			var setting = objectService.ConfigSetting.GetChildSetting(xpath);
			if(setting == null && attribute.Required) {
				throw new Exception($"必须为 {type.FullName} 配置节点：{xpath}");
			}
			IConfigSettingElement element;
			if (type.IsInterface) {//动态生成
				element = ConfigSettingElement.CreateProxy(setting, type);
			} else {
				element = (IConfigSettingElement)TypeHelper.CreateObject(type, typeof(IConfigSettingElement), false, args);
			}
			if(element != null) {
				element.ConfigSetting = setting;
			}
			ctx.ObjectInstance = element;
		}
	}
}

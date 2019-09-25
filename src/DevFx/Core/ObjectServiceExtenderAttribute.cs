using DevFx.Utils;
using System;
using System.Collections;
using System.Linq;

namespace DevFx.Core
{
	/// <summary>
	/// 定义<see cref="IObjectService"/>的扩展器
	/// </summary>
	[Serializable, AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
	public sealed class ObjectServiceExtenderAttribute : ServiceExtenderAttribute
	{
		//处理定义在类上的扩展器
		internal static void ExtenderInit(IObjectServiceContext ctx)  {
			var attributes = ctx.CoreAttributes;
			if (!attributes.TryGetValue(typeof(ObjectServiceExtenderAttribute), out var list)) {
				return;
			}
			if (list == null || list.Count <= 0) {
				return;
			}
			var items = ctx.Items;
			var instance = ctx.ObjectService;
			var orderList = list.OrderByDescending(x => x.Priority);
			foreach (ObjectServiceExtenderAttribute attribute in orderList) {
				var type = attribute?.OwnerType;
				if (type != null) {
					var extender = (IObjectExtender<IObjectService>)TypeHelper.CreateObject(type, typeof(IObjectExtender<IObjectService>), false);
					extender?.Init(instance, items);
				}
			}
		}
	}

	[Serializable, AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
	public abstract class ServiceExtenderAttribute : CoreAttribute
	{
		//处理定义在类上的扩展器
		public static void ExtenderInit<T, TA>(IObjectService objectService, T instance, IDictionary items = null) where TA : ServiceExtenderAttribute {
			var attributes = objectService.AsObjectServiceInternal().CoreAttributes;
			if (!attributes.TryGetValue(typeof(TA), out var list)) {
				return;
			}
			if(list == null || list.Count <= 0) {
				return;
			}
			items = items ?? new Hashtable();
			var orderList = list.OrderByDescending(x => x.Priority);
			foreach (TA attribute in orderList) {
				var type = attribute?.OwnerType;
				if (type != null) {
					var extender = objectService.GetOrCreateObject(type) as IObjectExtender<T>;
					extender?.Init(instance, items);
				}
			}
		}
	}
}
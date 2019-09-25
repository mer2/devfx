using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using DevFx.Core;
using DevFx.Core.Lifetime;
using DevFx.Reflection;
using DevFx.Utils;

namespace DevFx.Data
{
	[ObjectServiceExtender]
	internal class DataObjectServiceExtender : IObjectExtender<IObjectService>
	{
		public void Init(IObjectService instance, IDictionary items) {
			this.ObjectBuilder = new DataServiceBuilder(instance);
			instance.ObjectContainerGetted += this.OnObjectContainerGetted;
		}
		private DataServiceBuilder ObjectBuilder { get; set; }

		private void OnObjectContainerGetted(IObjectContainerContext ctx) {
			if (ctx.Container != null) {
				return;
			}
			var objectKey = ctx.ObjectKey;
			if (!(objectKey is Type)) {
				return;
			}
			var objectType = (Type)objectKey;
			if (!objectType.IsDefined(typeof(DataServiceAttribute), true)) {
				return;
			}
			var attribute = objectType.GetCustomAttribute<DataServiceAttribute>();
			//数据访问库没有定义，则自动生成
			IObjectContainer container = new SingletonObjectContainer();
			container.Init(new ObjectDescription(objectType, new Hashtable { { typeof(DataServiceAttribute), attribute } }), this.ObjectBuilder);
			ctx.Namespace.AddObject(objectType, container);
			ctx.Container = container;
		}

		internal class DataServiceBuilder : ObjectBuilderBase
		{
			public DataServiceBuilder(IObjectService objectService) : base(objectService) {
			}

			protected override void CreateObjectInternal(IObjectBuilderContext ctx, params object[] args) {
				var attribute = ctx.ObjectDescription.Items[typeof(DataServiceAttribute)] as DataServiceAttribute;
				var groupName = attribute?.GroupName;
				if(string.IsNullOrEmpty(groupName)) {
					groupName = ctx.ObjectDescription.ObjectType.FullName;
				}
				if(!string.IsNullOrEmpty(groupName)) {
					groupName += ".";
				}
				var instance = TypeHelper.CreateServiceDispatchProxy(ctx.ObjectDescription.ObjectType, new DataServiceWrapper { GroupName = groupName });
				ctx.ObjectInstance = instance;
			}
		}
	}

	internal class DataServiceWrapper : DataExecutorBase, IServiceDispatchProxy
	{
		public virtual string GroupName { get; set; }
		public virtual object Invoke(MethodInfo targetMethod, object[] args) {
			var declareType = targetMethod.DeclaringType;
			if (declareType == typeof(ISessionDataService)) {
				((ISessionDataService)this).SetDataSession((IDataSession)args[0], (bool)args[1]);
				return null;
			}
			if (declareType == typeof(IDisposable)) {
				((IDisposable)this).Dispose();
				return null;
			}
			var statementName = $"{this.GroupName}{targetMethod.Name}";
			object parameters = GetParameters(targetMethod, args);
			var resultType = targetMethod.ReturnType;
			var dop = this.GetDataOperation();
			if (!((IDataServiceInternal)DataService.Current).StatementExists(statementName)) {
				//从方法的Attribute上查找是否有定义sql
				var attribute = targetMethod.GetCustomAttribute<DataSqlAttribute>();
				if (attribute == null) {
					throw new DataException("指定的StatementName不存在：" + statementName);
				}
				var sql = attribute.Sql;
				var storage = attribute.StorageName;
				return dop.ExecuteSql(sql, parameters, storage, resultType, attribute.SqlType);
			}
			return dop.Execute(statementName, parameters, resultType);
		}

		protected static object GetParameters(MethodInfo targetMethod, object[] args) {
			var parameterInfos = targetMethod.GetParameters();
			if (parameterInfos != null && parameterInfos.Length > 0) {
				if (parameterInfos.Length == 1) {
					var parameter = parameterInfos[0];
					var parameterType = parameter.ParameterType;
					if (parameterType.IsClass && parameterType != typeof(string)) {
						return args[0];
					}
				}
				var parameters = new Dictionary<string, object>();
				for (var i = 0; i < parameterInfos.Length; i++) {
					var parameter = parameterInfos[i];
					parameters.Add(parameter.Name, args[i]);
				}
				return parameters;
			}
			return null;
		}
	}
}
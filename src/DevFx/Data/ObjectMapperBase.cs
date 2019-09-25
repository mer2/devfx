/* Copyright(c) 2005-2011 R2@DevFx.NET, License(LGPL) */

using System;
using System.Collections;
using DevFx.Core;
using DevFx.Data.Settings;
using DevFx.Utils;

namespace DevFx.Data
{
	public abstract class ObjectMapperBase : IObjectMapper
	{
		protected virtual IObjectService ObjectService => DevFx.ObjectService.Current;

		#region ObjectMapping

		protected virtual object MappingInternal(IStatementSetting statement, IDictionary dictionary, Type instanceType, object instance) {
			var resultType = statement.ResultTypeName;
			return this.GetOrCreateObject(resultType, instanceType, x => ObjectMapper.RefectionMapping(x, dictionary));
		}

		private object GetOrCreateObject(string objectAlias, Type instanceType, Action<object> initializer) {
			var objectService = this.ObjectService;
			var aliasNull = string.IsNullOrEmpty(objectAlias);
			var instance = aliasNull ? objectService.GetObject(instanceType) : objectService.GetObject(objectAlias);
			if (instance == null) {
				if (!aliasNull) {
					instance = objectService.GetOrCreateObject(objectAlias);
				}
				if (instance == null) {
					instance = TypeHelper.CreateObject(instanceType, null, false);
				}
				initializer?.Invoke(instance);
			}
			return instance;
		}

		#endregion

		#region IObjectMapper Members

		object IObjectMapper.Mapping(IStatementSetting statement, IDictionary dictionary, Type instanceType, object instance) {
			return this.MappingInternal(statement, dictionary, instanceType, instance);
		}

		#endregion
	}
}

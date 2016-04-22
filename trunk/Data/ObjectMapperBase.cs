/* Copyright(c) 2005-2011 R2@DevFx.NET, License(LGPL) */

using System;
using System.Collections;
using System.Data;
using System.Collections.Generic;
using System.Collections.Specialized;
using HTB.DevFx.Config;
using HTB.DevFx.Core;
using HTB.DevFx.Data.Config;
using HTB.DevFx.Data.Entities;
using HTB.DevFx.Utils;

namespace HTB.DevFx.Data
{
	public abstract class ObjectMapperBase : IObjectMapper
	{
		protected virtual IObjectService ObjectService {
			get { return DevFx.ObjectService.Current; }
		}

		#region ObjectMapping

		protected virtual object MappingInternal(IStatementSetting statement, IDictionary dictionary, Type instanceType, object instance) {
			var parameters = new HybridDictionary { { typeof(IObjectMapper), dictionary } };
			if(instance != null) {
				parameters.Add(typeof(ObjectMapperExtender), instance);
			}
			var resultType = statement.ResultTypeName;
			return this.GetOrCreateObject(resultType, instanceType, parameters, x => ObjectMapper.RefectionMapping(x, dictionary));
		}

		private object GetOrCreateObject(string objectAlias, Type instanceType, IDictionary parameters, Action<object> initializer) {
			var objectService = this.ObjectService;
			var aliasNull = string.IsNullOrEmpty(objectAlias);
			var instance = aliasNull ? objectService.GetObject(instanceType, parameters) : objectService.GetObject(objectAlias, parameters);
			if (instance == null) {
				if (!aliasNull) {
					var typeName = objectService.GetTypeName(objectAlias);
					if (!string.IsNullOrEmpty(typeName)) {
						instance = TypeHelper.CreateObject(typeName, instanceType, false);
					}
				}
				if (instance == null) {
					instance = TypeHelper.CreateObject(instanceType, null, false);
				}
				if (initializer != null) {
					initializer(instance);
				}
			}
			return instance;
		}

		#endregion

		#region IObjectMapper Members

		object IObjectMapper.Mapping(IStatementSetting statement, IDictionary dictionary, Type instanceType, object instance) {
			return this.MappingInternal(statement, dictionary, instanceType, instance);
		}

		#endregion

		#region ObjectMapperExtender
		
		protected internal class ObjectMapperExtender : ObjectServiceExtenderBase
		{
			protected override void Init(IObjectService objectService) {
				base.Init(objectService);
				objectService.ObjectBuilder.ObjectCreating += ObjectBuilderOnObjectCreating;
				objectService.ObjectBuilder.ObjectCreated += ObjectBuilderOnObjectCreated;
			}

			private void ObjectBuilderOnObjectCreating(IObjectBuilderContext builderContext) {
				var instance = builderContext.Items[typeof(ObjectMapperExtender)];
				if (instance == null) {
					return;
				}
				this.ObjectMappingInternal(builderContext, instance);
				builderContext.ObjectInstance = instance;
			}

			private void ObjectBuilderOnObjectCreated(IObjectBuilderContext builderContext) {
				var instance = builderContext.ObjectInstance;
				if (instance == null) {
					return;
				}
				this.ObjectMappingInternal(builderContext, instance);
			}

			private void ObjectMappingInternal(IObjectBuilderContext builderContext, object instance) {
				var dict = builderContext.Items[typeof(IObjectMapper)];
				if (dict == null || (!(dict is IDataRecord) && !(dict is IDictionary))) {
					return;
				}
	
				if(instance is IEntityInitialize) {
					((IEntityInitialize)instance).InitValues(dict is IDataRecord ? ((IDataRecord)dict).ToDictionary() : (IDictionary)dict);
					return;
				}
		
				var setting = builderContext.ObjectSetting.ConfigSetting;
				var mapSetting = (IObjectMapSetting)setting.ToCachedSetting<ObjectMapSetting>("objectMap");
				if (mapSetting == null) {
					return;
				}

				IEqualityComparer<string> comparer = null;
				if(mapSetting.IgnoreCase) {
					comparer = StringComparer.InvariantCultureIgnoreCase;
				}
				var dictionary = dict is IDataRecord ? ((IDataRecord)dict).ToDictionary(comparer) : ((IDictionary)dict).ToDictionary(comparer);

				var excludeProperties = new List<string>();
				var excludeString = mapSetting.ExcludeProperties;
				if (excludeString != null) {
					excludeProperties.AddRange(excludeString.Split(','));
				}
				var includeProperties = new List<string>();
				var includeString = mapSetting.IncludeProperties;
				includeProperties.AddRange(includeString.Split(','));

				var properties = mapSetting.Properties;
				if (properties != null && properties.Length > 0) {
					var translatorName = mapSetting.TypeTranslatorName;
					ITypeTranslator translator = null;
					if(!string.IsNullOrEmpty(translatorName)) {
						translator = this.ObjectService.GetOrCreateObject<ITypeTranslator>(translatorName);
					}
					var propertyInitilize = instance as IPropertyInitialize;
					foreach (var property in properties) {
						var columnName = property.ColumnName;
						if(!dictionary.ContainsKey(columnName)) {
							continue;
						}
						var value = dictionary[columnName];
						if(propertyInitilize != null) {
							propertyInitilize.SetValue(property.PropertyName, value);
						} else if (value != null) {
							if(translator != null) {
								ReflectionHelper.SetPropertyValue(instance, property.PropertyName, (x, p) => translator.Translate(value, p.MemberType, null));
							} else {
								ReflectionHelper.SetPropertyValue(instance, property.PropertyName, value);
							}
						}
						excludeProperties.Add(columnName);
						includeProperties.Remove(columnName);
					}
				}
				if (includeProperties.Count > 0) {
					ObjectMapper.RefectionMapping(instance, dictionary, excludeProperties, includeProperties);
				}
			}
		}

		#endregion
	}
}

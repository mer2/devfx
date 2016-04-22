/* Copyright(c) 2005-2011 R2@DevFx.NET, License(LGPL) */

using System;
using System.Collections;
using HTB.DevFx.Config;

namespace HTB.DevFx.Core
{
	partial class ObjectServiceBase : IObjectService
	{
		#region IObjectService 成员

		string IObjectService.GetTypeName(string typeAlias) {
			return this.GetTypeNameInternal(typeAlias);
		}

		T IObjectService.GetObject<T>() {
			return this.GetObjectInternal<T>(null);
		}

		T IObjectService.CreateObject<T>(params object[] args) {
			return this.CreateObjectInternal<T>(null, args);
		}

		object IObjectService.GetObject(Type type) {
			return this.GetObjectInternal(type, null, null);
		}

		object IObjectService.CreateObject(Type type, params object[] args) {
			return this.CreateObjectInternal(type, null, null, args);
		}

		IConfigSetting IObjectService.GetObjectSetting<T>() {
			return this.GetObjectSettingInternal(typeof(T));
		}

		IConfigSetting IObjectService.GetObjectSetting(Type type) {
			return this.GetObjectSettingInternal(type);
		}

		object IObjectService.GetObject(string objectAlias) {
			return this.GetObjectInternal(objectAlias, null);
		}

		T IObjectService.GetObject<T>(string objectAlias) {
			return (T)this.GetObjectInternal(objectAlias, null);
		}

		object IObjectService.CreateObject(string objectAlias, params object[] args) {
			return this.CreateObjectInternal(objectAlias, null, null, args);
		}

		IConfigSetting IObjectService.GetObjectSetting(string objectAlias) {
			return this.GetObjectSettingInternal(objectAlias);
		}

		T[] IObjectService.GetObjects<T>() {
			return this.GetObjectsInternal<T>(null);
		}

		object[] IObjectService.GetObjects(Type type) {
			return this.GetObjectsInternal(type, null);
		}

		IObjectBuilder IObjectService.ObjectBuilder {
			get { return this.ObjectBuilder; }
		}

		#region Advanced Usage

		object IObjectService.GetObject(string objectAlias, IDictionary items) {
			return this.GetObjectInternal(objectAlias, items);
		}

		T IObjectService.GetObject<T>(string objectAlias, IDictionary items) {
			return (T)this.GetObjectInternal(objectAlias, items);
		}

		T IObjectService.GetObject<T>(IDictionary items) {
			return this.GetObjectInternal<T>(items);
		}

		object IObjectService.GetObject(Type type, IDictionary items) {
			return this.GetObjectInternal(type, null, items);
		}

		T IObjectService.CreateObject<T>(IDictionary items, params object[] args) {
			return this.CreateObjectInternal<T>(items, args);
		}

		object IObjectService.CreateObject(Type type, IDictionary items, params object[] args) {
			return this.CreateObjectInternal(type, null, items, args);
		}

		object IObjectService.CreateObject(string objectAlias, IDictionary items, params object[] args) {
			return this.CreateObjectInternal(objectAlias, null, items, args);
		}

		T[] IObjectService.GetObjects<T>(IDictionary items) {
			return this.GetObjectsInternal<T>(items);
		}

		object[] IObjectService.GetObjects(Type type, IDictionary items) {
			return this.GetObjectsInternal(type, items);
		}

		#endregion

		#endregion
	}
}

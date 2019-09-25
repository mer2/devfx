/* Copyright(c) 2005-2011 R2@DevFx.NET, License(LGPL) */

using System;

namespace DevFx.Core
{
	partial class ObjectServiceBase : IObjectService
	{
		#region IObjectService 成员

		T IObjectService.GetObject<T>() {
			return this.GetObjectInternal<T>(null);
		}

		T IObjectService.GetObject<T>(string name) {
			return (T)this.GetObjectInternal(name, null);
		}

		object IObjectService.GetObject(Type type) {
			return this.GetObjectInternal(type, null, null);
		}

		object IObjectService.GetObject(string name) {
			return this.GetObjectInternal(name, null);
		}

		Type IObjectService.GetType(string typeAlias) {
			return this.GetTypeInternal(typeAlias);
		}

		T[] IObjectService.GetObjects<T>() {
			return this.GetObjectsInternal<T>(null);
		}

		object[] IObjectService.GetObjects(Type type) {
			return this.GetObjectsInternal(type, null);
		}

		T IObjectService.CreateObject<T>(params object[] args) {
			return this.CreateObjectInternal<T>(null, args);
		}

		object IObjectService.CreateObject(Type type, params object[] args) {
			return this.CreateObjectInternal(type, null, null, args);
		}

		IObjectBuilder IObjectService.ObjectBuilder => this.ObjectBuilder;

		#endregion
	}
}

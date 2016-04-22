/* Copyright(c) 2005-2011 R2@DevFx.NET, License(LGPL) */

using System;
using System.Collections;
using System.Collections.Generic;

namespace HTB.DevFx.Data.Entities
{
	[Serializable]
	public abstract class EntityBase<TEntity> : IEntity, IPropertyInitialize, IEntityInitialize where TEntity : EntityBase<TEntity>
	{
		protected internal virtual void SetDirty(string propertyName, object propertyValue) {
			this.GetChangedValues()[propertyName] = propertyValue;
		}

		[NonSerialized]
		private Dictionary<string, object> changedValues;
		protected virtual Dictionary<string, object> GetChangedValues() {
			return this.changedValues ?? (this.changedValues = new Dictionary<string, object>());
		}

		protected virtual void InitValues(IDictionary values) {
		}

		protected virtual void SetValue(string propertyName, object propertyValue) {
		}

		#region IEntity Members

		void IEntityInitialize.InitValues(IDictionary values) {
			this.InitValues(values);
		}

		IDictionary IEntity.GetChangedValues() {
			return this.GetChangedValues();
		}
	
		#endregion

		#region IPropertyInitialize Members

		void IPropertyInitialize.SetValue(string propertyName, object propertyValue) {
			this.SetValue(propertyName, propertyValue);
		}

		#endregion
	}
}

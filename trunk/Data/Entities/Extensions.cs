/* Copyright(c) 2005-2011 R2@DevFx.NET, License(LGPL) */

using System;
using System.Linq.Expressions;
using System.Collections;

namespace HTB.DevFx.Data.Entities
{
	public static class Extensions
	{
		public static TEntity OnDirty<TEntity>(this TEntity entity, Func<TEntity, bool> dirty) where TEntity : EntityBase<TEntity> {
			if(entity != null) {
				if(!dirty(entity)) {
					entity = null;
				}
			}
			return entity;
		}

		public static TEntity SetValue<TEntity, TValue>(this TEntity entity, Action<TEntity, TValue> setter, TValue newValue) where TEntity : EntityBase<TEntity> {
			if(entity != null) {
				setter(entity, newValue);
			}
			return entity;
		}

		public static TEntity SetValue<TEntity>(this TEntity entity, Action<TEntity> setter) where TEntity : EntityBase<TEntity> {
			if (entity != null) {
				setter(entity);
			}
			return entity;
		}

		public static TEntity SetDirty<TEntity, TValue>(this TEntity entity, Expression<Func<TEntity, TValue>> propertySelector) where TEntity : EntityBase<TEntity> {
			if (entity != null) {
				var propertyName = ((MemberExpression)propertySelector.Body).Member.Name;
				entity.SetDirty(propertyName, propertySelector.Compile()(entity));
			}
			return entity;
		}

		public static TEntity SetDirty<TEntity, TValue>(this TEntity entity, Expression<Func<TEntity, TValue>> propertySelector, TValue newValue) where TEntity : EntityBase<TEntity> {
			if (entity != null) {
				var propertyName = ((MemberExpression)propertySelector.Body).Member.Name;
				entity.SetDirty(propertyName, newValue);
			}
			return entity;
		}

		public static bool TryGetValue<T>(this IDictionary values, string propertyName, out T propertyValue) {
			if(values != null) {
				if(values.Contains(propertyName)) {
					var value = values[propertyName];
					if(!Convert.IsDBNull(value)) {
						propertyValue = (T)values[propertyName];
						return true;
					}
				}
			}
			propertyValue = default(T);
			return false;
		}

		public static T TryGetValue<T>(this IDictionary values, string propertyName) {
			T value;
			values.TryGetValue(propertyName, out value);
			return value;
		}

		public static T TryGetValue<T>(this IDictionary values, string propertyName, T defaultValue) {
			T value;
			if(!values.TryGetValue(propertyName, out value)) {
				value = defaultValue;
			}
			return value;
		}
	}
}

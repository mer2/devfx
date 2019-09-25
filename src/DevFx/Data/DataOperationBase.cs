/* Copyright(c) 2005-2011 R2@DevFx.NET, License(LGPL) */

using DevFx.Utils;
using System;
using System.Collections;
using System.Data;

namespace DevFx.Data
{
	public abstract partial class DataOperationBase
	{
		protected DataOperationBase(IResultHandlerFactory handlerFactory) {
			this.ResultHandlerFactory = handlerFactory;
		}

		protected virtual IResultHandlerFactory ResultHandlerFactory { get; }

		#region DbExecuteContex

		protected abstract IDbExecuteContext GetExecuteContext(string statementName, IDictionary parameters);
		protected abstract IDbExecuteContext GetSqlExecuteContext(string sql, IDictionary parameters, string storageName = null, CommandType sqlType = CommandType.Text);

		#endregion

		#region Execute

		protected virtual object Execute(string statementName, object parameters, Type resultType) {
			return this.ExecuteInternal(statementName, parameters.ToDictionary(), null, resultType);
		}

		protected virtual T Execute<T>(string statementName, object parameters) {
			return this.ExecuteInternal(statementName, parameters.ToDictionary(), default(T));
		}

		protected virtual object Execute(string statementName, object parameters, object result) {
			return this.ExecuteInternal(statementName, parameters.ToDictionary(), result);
		}

		protected virtual T Execute<T>(string statementName, object parameters, T result) {
			return this.ExecuteInternal(statementName, parameters.ToDictionary(), result);
		}

		protected virtual T ExecuteInternal<T>(string statementName, IDictionary parameters, T result) {
			using (var ctx = this.GetExecuteContext(statementName, parameters)) {
				var returnValue = this.ExecuteResult(ctx, typeof(T), result);
				return (T) returnValue;
			}
		}

		protected virtual object ExecuteInternal(string statementName, IDictionary parameters, object result, Type resultType) {
			using (var ctx = this.GetExecuteContext(statementName, parameters)) {
				return this.ExecuteResult(ctx, resultType, result);
			}
		}

		protected virtual object ExecuteResult(IDbExecuteContext ctx, Type resultType, object resultInstance) {
			return this.ResultHandlerFactory.ExecuteResult(ctx, resultType, resultInstance);
		}

		#endregion

		#region ExecuteSql

		protected virtual object ExecuteSql(string sql, object parameters, string storageName = null, Type resultType = null, CommandType sqlType = CommandType.Text) {
			return this.ExecuteSqlInternal(sql, parameters.ToDictionary(), null, storageName, resultType, sqlType);
		}

		protected virtual T ExecuteSql<T>(string sql, object parameters, string storageName = null, CommandType sqlType = CommandType.Text) {
			return this.ExecuteSqlInternal(sql, parameters.ToDictionary(), default(T), storageName, sqlType);
		}

		protected virtual object ExecuteSqlInternal(string sql, IDictionary parameters, object result, string storageName = null, Type resultType = null, CommandType sqlType = CommandType.Text) {
			using(var ctx = this.GetSqlExecuteContext(sql, parameters, storageName, sqlType)) {
				return this.ExecuteResult(ctx, resultType, result);
			}
		}

		protected virtual T ExecuteSqlInternal<T>(string sql, IDictionary parameters, T result, string storageName = null, CommandType sqlType = CommandType.Text) {
			using(var ctx = this.GetSqlExecuteContext(sql, parameters, storageName, sqlType)) {
				var returnValue = this.ExecuteResult(ctx, typeof(T), result);
				return (T)returnValue;
			}
		}

		#endregion
	}
}

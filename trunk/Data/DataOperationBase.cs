/* Copyright(c) 2005-2011 R2@DevFx.NET, License(LGPL) */

using System;
using System.Collections;

namespace HTB.DevFx.Data
{
	public abstract partial class DataOperationBase
	{
		protected DataOperationBase(IResultHandlerFactory handlerFactory) {
			this.ResultHandlerFactory = handlerFactory;
		}

		protected virtual IResultHandlerFactory ResultHandlerFactory { get; private set; }

		#region DbExecuteContex

		protected abstract IDbExecuteContext GetExecuteContext(string statementName, IDictionary parameters);

		#endregion
	
		#region Execute

		protected virtual object Execute(string statementName, object parameters) {
			return this.ExecuteInternal(statementName, parameters.ToDictionary(), null);
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
				return returnValue == null ? default(T) : (T)returnValue;
			}
		}

		protected virtual object ExecuteInternal(string statementName, IDictionary parameters, object result) {
			using (var ctx = this.GetExecuteContext(statementName, parameters)) {
				return this.ExecuteResult(ctx, null, result);
			}
		}

		protected virtual object ExecuteResult(IDbExecuteContext ctx, Type resultType, object resultInstance) {
			return this.ResultHandlerFactory.ExecuteResult(ctx, resultType, resultInstance);
		}

		#endregion
	}
}

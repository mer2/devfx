/* Copyright(c) 2005-2011 R2@DevFx.NET, License(LGPL) */

using System;
using System.Data;

namespace DevFx.Data
{
	/// <summary>
	/// 数据存储操作接口
	/// </summary>
	public interface IDataOperation
	{
		/// <summary>
		/// 执行指定数据语句
		/// </summary>
		/// <param name="statementName">数据语句名</param>
		/// <param name="parameters">参数</param>
		/// <param name="resultType">返回结果参数，为null则根据配置而定</param>
		/// <returns>执行结果</returns>
		object Execute(string statementName, object parameters, Type resultType = null);

		/// <summary>
		/// 执行指定数据语句，并返回强类型（实体）
		/// </summary>
		/// <typeparam name="T">实体类型</typeparam>
		/// <param name="statementName">数据语句名</param>
		/// <param name="parameters">参数</param>
		/// <returns>强类型（实体）</returns>
		T Execute<T>(string statementName, object parameters);

		/// <summary>
		/// 执行指定Sql语句
		/// </summary>
		/// <param name="sql">Sql语句</param>
		/// <param name="parameters">参数</param>
		/// <param name="storageName">所属存储名</param>
		/// <param name="resultType">返回结果参数，为null则根据配置而定</param>
		/// <param name="sqlType">Sql类型</param>
		/// <returns>执行结果</returns>
		object ExecuteSql(string sql, object parameters, string storageName = null, Type resultType = null, CommandType sqlType = CommandType.Text);

		/// <summary>
		/// 执行指定Sql语句，并返回强类型（实体）
		/// </summary>
		/// <typeparam name="T">实体类型</typeparam>
		/// <param name="sql">Sql语句</param>
		/// <param name="parameters">参数</param>
		/// <param name="storageName">所属存储名</param>
		/// <param name="sqlType">Sql类型</param>
		/// <returns>强类型（实体）</returns>
		T ExecuteSql<T>(string sql, object parameters, string storageName = null, CommandType sqlType = CommandType.Text);
	}
}

/* Copyright(c) 2005-2011 R2@DevFx.NET, License(LGPL) */

namespace HTB.DevFx.Data
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
		/// <returns>执行结果（根据配置而定）</returns>
		object Execute(string statementName, object parameters);

		/// <summary>
		/// 执行指定数据语句，并返回强类型（实体）
		/// </summary>
		/// <typeparam name="T">实体类型</typeparam>
		/// <param name="statementName">数据语句名</param>
		/// <param name="parameters">参数</param>
		/// <returns>强类型（实体）</returns>
		T Execute<T>(string statementName, object parameters);

		/// <summary>
		/// 执行指定数据语句，返回结果填入<paramref name="result"/>中
		/// </summary>
		/// <param name="statementName">数据语句名</param>
		/// <param name="parameters">参数</param>
		/// <param name="result">返回结果实体</param>
		/// <returns>返回结果实体，一般是<paramref name="result"/></returns>
		object Execute(string statementName, object parameters, object result);

		/// <summary>
		/// 执行指定数据语句，返回结果填入强类型<paramref name="result"/>中
		/// </summary>
		/// <typeparam name="T">结果类型</typeparam>
		/// <param name="statementName">数据语句名</param>
		/// <param name="parameters">参数</param>
		/// <param name="result">返回结果实体</param>
		/// <returns>返回结果实体，一般是<paramref name="result"/></returns>
		T Execute<T>(string statementName, object parameters, T result);
	}
}

namespace DevFx.Logging
{
	/// <summary>
	/// 日志记录器接口
	/// </summary>
	public interface ILogger
	{
		/// <summary>
		/// 批量写入日志
		/// </summary>
		/// <param name="items"><see cref="LogItem"/> 数组</param>
		void WriteLogs(LogItem[] items);
	}
}
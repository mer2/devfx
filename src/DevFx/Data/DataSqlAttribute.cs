using System;
using System.Data;

namespace DevFx.Data
{
	[Serializable, AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
	public class DataSqlAttribute : Attribute
	{
		public DataSqlAttribute(string sql) {
			if(string.IsNullOrEmpty(sql)) {
				throw new ArgumentNullException(nameof(sql));
			}
			this.Sql = sql;
		}

		public string Sql { get; set; }
		public CommandType SqlType { get; set; } = CommandType.Text;
		public string StorageName { get; set; }
	}
}
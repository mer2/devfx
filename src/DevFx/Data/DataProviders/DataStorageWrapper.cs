using System.Data.Common;
using DevFx.Data.Settings;

namespace DevFx.Data.DataProviders
{
    internal class DataStorageWrapper<T> : DataStorageBase
	{
		protected override DbProviderFactory GetDbProviderFactory(IConnectionStringSetting connectionString) {
			var type = typeof(T);
			var field = type.GetField("Instance");
			if(field == null) {
				throw new DataException($"未能找到类型为 {type.FullName} 的静态字段：Instance");
			}
			var instance = field.GetValue(null) as DbProviderFactory;
			return instance;
		}
	}
}

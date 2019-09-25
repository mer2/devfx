using DevFx.Data.Results;
using DevFx.Utils;
using System;
using System.Collections;

namespace DevFx.Data.DataProviders
{
	public class SqlServerTypeTranslator : ITypeTranslator
	{
		public object Translate(object value, Type expectedType, IDictionary options) {
			if (value == null || value == DBNull.Value) {
				return expectedType.IsValueType ? TypeHelper.CreateObject(expectedType, null, true) : null;
			}

			var convertingValue = value;
			if (convertingValue is DateTime && expectedType == typeof(DateTime?)) {
				return convertingValue;
			}
			if (expectedType == null) {
				return convertingValue;
			}
			return expectedType.IsEnum ? Enum.ToObject(expectedType, Convert.ToInt32(value)) : Convert.ChangeType(convertingValue, expectedType);
		}
	}
}

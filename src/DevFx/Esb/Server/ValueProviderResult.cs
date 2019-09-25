using DevFx.Esb.Serialize;
using System;
using System.Collections;

namespace DevFx.Esb.Server
{
	public class ValueProviderResult
	{
		protected ValueProviderResult() {
		}

		public ValueProviderResult(object rawValue, ISerializer serializer) {
			RawValue = rawValue;
			this.Serializer = serializer;
		}

		public object RawValue { get; protected set; }
		public ISerializer Serializer { get; protected set; }

		public virtual object ConvertTo(Type type) {
			return this.ConvertTypeInternal(this.RawValue, type);
		}

		public T ConvertTo<T>() {
			return (T)this.ConvertTo(typeof (T));
		}

		protected virtual object ConvertTypeInternal(object value, Type destinationType) {
			if(value != null && destinationType.IsInstanceOfType(value)) {//类型匹配，直接返回
				return value;
			}
			if(destinationType.IsValueType && value is string && (string)value == string.Empty) {//期望的类型是值类型，则把空字符串换成null
				value = null;
			}
			var nullable = destinationType.IsGenericType && destinationType.GetGenericTypeDefinition() == typeof(Nullable<>);
			if(value == null) {
				if(destinationType.IsValueType && !nullable) {
					return Convert.ChangeType(0, destinationType);//值类型的默认值为0
				}
				return null;
			}
			if(destinationType.IsArray && value is ArrayList) {//数组
				var list = (ArrayList)value;
				return list.ToArray(destinationType.GetElementType());
			}
			if(nullable) {//可为空的值类型，获取真正的值类型
				destinationType = destinationType.GetGenericArguments()[0];
			}
			if (destinationType.IsPrimitive) {//如果是基础类型，则直接转换
				return Convert.ChangeType(value, destinationType);
			}
			//否则如果是非基础类型，尝试反序列化
			if(this.Serializer != null) {
				return this.Serializer.Convert(value, destinationType, null);
			}
			return Convert.ChangeType(value, destinationType);
		}
	}
}

using System.Collections.Generic;
using System.Reflection;

namespace DevFx.Utils
{
	partial class TypeHelper
    {
		private static readonly IDictionary<ICustomAttributeProvider, object> AttributesCache = new Dictionary<ICustomAttributeProvider, object>();
		private static readonly IDictionary<ICustomAttributeProvider, object> InheritAttributesCache = new Dictionary<ICustomAttributeProvider, object>();

	    public static T[] GetCustomAttributes<T>(ICustomAttributeProvider type, bool inherit) {
		    if (type == null) {
			    return new T[0];
		    }
		    var dict = inherit ? InheritAttributesCache : AttributesCache;
		    if (dict.TryGetValue(type, out var attributes)) {
			    return (T[])attributes;
		    }
		    lock (dict) {
			    if(dict.TryGetValue(type, out attributes)) {
				    return (T[])attributes;
			    }
			    var atts = type.GetCustomAttributes(typeof(T), inherit) as T[] ?? new T[0];
			    dict[type] = atts;
				return atts;
			}
		}
	}
}

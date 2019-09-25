using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace DevFx.Esb.Server
{
	internal class ValueProviderCollection : Collection<IValueProvider>, IValueProvider
	{
		public ValueProviderCollection() {
		}

		public ValueProviderCollection(IList<IValueProvider> list) : base(list) {
		}

		public ValueProviderResult GetValue(string name) {
			return (from provider in this
					let result = provider.GetValue(name)
					where result != null
					select result).FirstOrDefault();
		}
	}
}

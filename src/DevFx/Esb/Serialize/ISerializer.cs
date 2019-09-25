using System;
using System.Collections;
using System.IO;

namespace DevFx.Esb.Serialize
{
	public interface ISerializer
	{
		void Serialize(Stream stream, object instance, IDictionary options);
		object Deserialize(Stream stream, Type expectedType, IDictionary options);
		object Convert(object instance, Type expectedType, IDictionary options);
		string ContentType { get; set; }
	}
}

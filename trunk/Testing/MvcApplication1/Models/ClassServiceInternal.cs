using System;
using ClassLibrary1;

namespace MvcApplication1.Models
{
	internal class ClassServiceInternal : IClassService, ClassLibrary2.IClassService
	{
		public string HelloWorld() {
			return DateTime.Now.ToString();
		}
	}
}
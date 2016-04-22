using System;
using System.Collections.Generic;
using System.Text;
using ClassLibrary1;
using HTB.DevFx.Esb;

namespace ConsoleApplication2
{
	class Program
	{
		static void Main(string[] args) {
			var svr = ServiceLocator.GetService<IClassService>();
			Console.WriteLine(svr.HelloWorld());
			Console.ReadLine();
		}
	}
}

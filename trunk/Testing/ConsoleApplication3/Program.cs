using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using ClassLibrary2;
using HTB.DevFx.Remoting;
using HTB.DevFx.Utils;

namespace ConsoleApplication3
{
	class Program
	{
		static void Main(string[] args) {
			//RemotingHelper.RemotingServiceInitialize();
			//Console.WriteLine("Ready....");

			var type = TypeHelper.CreateObject("ConsoleApplication3.ClassServiceInternal, ConsoleApplication3", null, false);
			type.ToString();
			Console.ReadLine();
		}
	}

	internal class ClassServiceInternal : IClassService
	{
		public string HelloWorld() {
			return DateTime.Now.ToString();
		}
	}
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ClassLibrary2;
using HTB.DevFx;
using HTB.DevFx.Log;

namespace ConsoleApplication4
{
	class Program
	{
		static void Main(string[] args) {
			var svr = ObjectService.GetObject<ILogService>();
			Console.WriteLine(svr.ToString());
			Console.ReadLine();
		}
	}
}

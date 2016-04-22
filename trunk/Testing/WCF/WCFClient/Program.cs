using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ClassLibrary2;
using HTB.DevFx;

namespace WCFClient
{
    class Program
    {
        static void Main(string[] args)
        {
            var svr = ObjectService.GetObject<IClassService>();
            Console.WriteLine(svr.HelloWorld());
            Console.ReadLine();
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ClassLibrary1;
using HTB.DevFx.Esb;

namespace RemotingClient
{
    class Program
    {
        static void Main(string[] args)
        {
            var svr = ServiceLocator.GetService<IClassService>();
            Console.WriteLine(svr.HelloWorld());
            Console.ReadLine();
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ClassLibrary1;
using HTB.DevFx.Remoting;

namespace RemotingServer
{
    class Program
    {
        static void Main(string[] args)
        {
            RemotingHelper.RemotingServiceInitialize();
            Console.WriteLine("Ready....");
            Console.ReadLine();
        }
    }


    internal class ClassServiceInternal : IClassService
    {
        public string HelloWorld()
        {
            return "Has received(Remoting)！Time:" + DateTime.Now.ToString();
        }
    }
}

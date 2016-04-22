using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ClassLibrary2;
using HTB.DevFx.Remoting;

namespace WCFServer
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
            return "Has received(WCF)！Time:" + DateTime.Now.ToString();
        }
    }
}

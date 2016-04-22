using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Reflection.Emit;
using System.Threading;
using System.Web;
using ClassLibrary1;
using HTB.DevFx;
using HTB.DevFx.Core;
using HTB.DevFx.Remoting;
using HTB.DevFx.Utils;

namespace ConsoleApplication1
{
	class Program
	{
		static void Main(string[] args) {
			//var domain = Thread.GetDomain();
			//var name = new AssemblyName("Octopus.Esb.Server.Mvc.DynamicControllers");
			//var assemblyBuilder = domain.DefineDynamicAssembly(name, AssemblyBuilderAccess.RunAndSave, domain.SetupInformation.ApplicationBase);
			//var moduleBuilder = assemblyBuilder.DefineDynamicModule("DynamicModule", "dy.dll");
			//var tb = TypeHelper.CreateDynamic(moduleBuilder, typeof(IClassService));
			//tb.CreateType();
			//assemblyBuilder.Save("dy.dll");

			//RemotingHelper.RemotingServiceInitialize();
			//Console.WriteLine("Ready....");
			//var type = TypeHelper.CreateObject("ConsoleApplication1.ClassServiceInternal, ConsoleApplication1", null, false);
			//type.ToString();
			//var path = "..\\Logs\\{CurrentDirectoryName}";
			//if(path.IndexOf("{CurrentDirectoryName}", StringComparison.InvariantCulture) >= 0) {
			//    //需要替换{CurrentDirectory}为当前目录名
			//    var appPath = AppDomain.CurrentDomain.SetupInformation.ApplicationBase;
			//    var directoryName = string.Empty;
			//    var directory = new DirectoryInfo(appPath);
			//    if(directory.Root.Name != directory.Name) {
			//        directoryName = directory.Name;
			//    }
			//    path = path.Replace("{CurrentDirectoryName}", directoryName);
			//}
			//Console.WriteLine(FileHelper.GetFullPath(path + "\\"));
			var fileName = GetFileName(@"..\Logs\Error\{0:yyyy}-{0:MM}-{0:dd}\{2}\" + "{1}.log");
			FileHelper.WriteLog(fileName, "error");
			Console.ReadLine();
		}

		protected static string GetFileName(string input, Func<string, string> filter = null) {
			//需要替换{0}为当前时间
			var now = DateTime.Now;
			//需要替换{1}为当前服务器名，目前为当前服务器IP
			var serverName = Environment.MachineName;
			if(filter != null) {
				serverName = filter(serverName);
			}
			//需要替换{2}为当前目录名
			var appPath = AppDomain.CurrentDomain.SetupInformation.ApplicationBase;
			var directoryName = string.Empty;
			var directory = new DirectoryInfo(appPath);
			if(directory.Root.Name != directory.Name) {
				directoryName = directory.Name;
			}
			if(filter != null) {
				directoryName = filter(directoryName);
			}
			var filePath = string.Format(input, now, serverName, directoryName);
			return FileHelper.GetFullPath(filePath);
		}
	}

	internal class ClassServiceInternal : IClassService
	{
		static ClassServiceInternal() {
			Console.WriteLine("ClassServiceInternal static ctor.");
		}
		public string HelloWorld() {
			return DateTime.Now.ToString();
		}
	}

	internal abstract class ServiceHandleProxy<TContract> : IHttpHandler
	{
		protected abstract TContract ProxyInstance { get; }
		protected abstract IDictionary<string, object> GetCallParameters(HttpContext ctx);
		protected abstract MethodInfo GetCallMethod(HttpContext ctx, IDictionary<string, object> parameters);

		protected virtual void ResultHandle(object result) {
		}

		public void ProcessRequest(HttpContext context) {
			var parameters = this.GetCallParameters(context);
			var method = this.GetCallMethod(context, parameters);
			object result;
			if (method != null) {
				try {
					TypeHelper.TryInvoke(this.ProxyInstance, method, out result, true, parameters.Values);
				} catch(Exception e) {
					result = AOPResult.Failed(e.Message);
				}
			} else {
				result = AOPResult.Failed("method not found");
			}
			this.ResultHandle(result);
		}

		public bool IsReusable { get { return true; } }
	}
}

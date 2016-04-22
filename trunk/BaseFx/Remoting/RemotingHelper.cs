/* Copyright(c) 2005-2011 R2@DevFx.NET, License(LGPL) */

using System;
using System.Collections.Generic;
using System.Runtime.Remoting;
using HTB.DevFx.Esb;
using HTB.DevFx.Remoting.Web;

namespace HTB.DevFx.Remoting
{
	/// <summary>
	/// Remoting技术相关辅助类
	/// </summary>
	public static class RemotingHelper
	{
		private static readonly List<string> ConfigFiles = new List<string>();

		/// <summary>
		/// 使用配置文件注册远程对象，防止Remoting对象重复注册
		/// </summary>
		/// <param name="configFile">远程对象的配置文件（全路径）</param>
		public static void RegisterRemotingObject(string configFile) {
			if(string.IsNullOrEmpty(configFile)) {
				configFile = AppDomain.CurrentDomain.SetupInformation.ConfigurationFile;
			}
			if (!string.IsNullOrEmpty(configFile)) {
				configFile = configFile.ToLower();
				if (!ConfigFiles.Contains(configFile)) {
					lock(typeof(RemotingHelper)) {
						if(!ConfigFiles.Contains(configFile)) {
							try {
								RemotingConfiguration.Configure(configFile, false);
								ConfigFiles.Add(configFile);
							} catch { }
						}
					}
				}
			}
		}

		/// <summary>
		/// 对已配置好的服务进行初始化
		/// </summary>
		/// <remarks>
		/// 必须在适当的时候调用此方法以保证服务被发布出去
		/// 在Web应用中可以采用替换*.rem的处理工厂方式，把<see cref="System.Runtime.Remoting.Channels.Http.HttpRemotingHandlerFactory"/>替换成<see cref="Web.HttpRemotingHandlerFactory"/>
		/// 或者添加一个HttpModule，类型为<see cref="RemotingServiceModule"/>
		/// </remarks>
		public static void RemotingServiceInitialize() {
			ServiceLocator.GetService<RemotingServiceInternal>();
		}
	}
}

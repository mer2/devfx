/* Copyright(c) 2005-2017 R2@DevFx.NET, License(LGPL) */

using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;

namespace DevFx.Utils
{
	/// <summary>
	/// 关于文件的一些实用方法
	/// </summary>
	public static class FileHelper
	{
		#region File

		/// <summary>
		/// 缺省的文件查找目录列表
		/// </summary>
		public static readonly string[] DefaultSearchPath = {
			"./",
			AppDomain.CurrentDomain.BaseDirectory
		};

		/// <summary>
		/// 验证文件路径并返回
		/// </summary>
		/// <param name="fileName">文件名</param>
		/// <param name="searchPath">搜索目录列表</param>
		/// <returns>返回文件路径</returns>
		public static string SearchFile(string fileName, params string[] searchPath) {
			if (File.Exists(fileName)) {
				return fileName;
			}
			if (searchPath == null || searchPath.Length <= 0) {
				searchPath = DefaultSearchPath;
			}
			foreach (var filePath in searchPath) {
				var fullName = Path.GetFullPath(filePath + fileName);
				if (File.Exists(fullName)) {
					return fullName;
				}
			}
			return null;
		}

		/// <summary>
		/// 查找由通配符指定的文件集合
		/// </summary>
		/// <param name="filePattern">文件通配符</param>
		/// <param name="searchPath">搜索目录列表</param>
		/// <returns>找到的文件列表</returns>
		public static string[] SearchFileWithPattern(string filePattern, params string[] searchPath) {
			return SearchFileWithPattern(filePattern, SearchOption.TopDirectoryOnly, searchPath);
		}

		/// <summary>
		/// 查找由通配符指定的文件集合
		/// </summary>
		/// <param name="filePattern">文件通配符</param>
		/// <param name="searchOption">查找选项</param>
		/// <param name="searchPath">搜索目录列表</param>
		/// <returns>找到的文件列表</returns>
		public static string[] SearchFileWithPattern(string filePattern, SearchOption searchOption, params string[] searchPath) {
			if (searchPath == null || searchPath.Length <= 0) {
				searchPath = DefaultSearchPath;
			}
			var foundFils = new List<string>();
			foreach (var filePath in searchPath) {
				var fullPath = Path.GetFullPath(filePath);
				if (Directory.Exists(fullPath)) {
					var files = Directory.GetFiles(fullPath, filePattern, searchOption);
					foundFils.AddRange(files);
				}
			}
			return foundFils.ToArray();
		}

		/// <summary>
		/// 分离程序集内嵌资源为资源名称和程序集名称：res://资源全名称, 所在程序集名称
		/// </summary>
		/// <param name="resourceUri">res://资源全名称, 所在程序集名称</param>
		/// <param name="resourceAssemblyName">所在程序集名称</param>
		/// <returns>资源全名称</returns>
		internal static string GetResourceName(string resourceUri, out string resourceAssemblyName) {
			resourceAssemblyName = null;
			if (string.IsNullOrEmpty(resourceUri)) {
				return null;
			}
			if (!resourceUri.StartsWith("res://", true, null)) {
				return null;
			}
			resourceUri = resourceUri.Substring(6);
			var index = resourceUri.IndexOf(',');
			if (index <= 0 || index >= (resourceUri.Length - 1)) {
				return resourceUri;
			}
			var resourceFullName = resourceUri.Substring(0, index).Trim();
			resourceAssemblyName = resourceUri.Substring(index + 1).Trim();
			return resourceFullName;
		}

		/// <summary>
		/// 查找并获取指定文件内容
		/// </summary>
		/// <param name="fileUri">文件的Uri</param>
		/// <returns>返回找到的 <see cref="Stream"/> ，如果Uri格式不正确或文件不存在则返回 <c>null</c></returns>
		/// <remarks>
		/// 文件的Uri
		///		<para>程序集内嵌资源：res://资源全名称, 所在程序集名称</para>
		///		<para>物理文件：file://全路径或相对路径</para>
		///		<para>网络文件（暂不支持）：http://网络地址</para>
		/// </remarks>
		public static Stream GetFileStream(string fileUri) {
			if (string.IsNullOrEmpty(fileUri)) {
				return null;
			}
			if (fileUri.StartsWith("res://", true, null)) {
				var resourceFullName = GetResourceName(fileUri, out var resourceAssemblyName);
				if (string.IsNullOrEmpty(resourceFullName) || string.IsNullOrEmpty(resourceAssemblyName)) {
					return null;
				}
				try {
					var resourceAssembly = Assembly.Load(resourceAssemblyName);
					if (resourceAssembly != null) {
						return resourceAssembly.GetManifestResourceStream(resourceFullName);
					}
				} catch {
					return null;
				}
			}
			if (fileUri.StartsWith("http://", true, null)) {
				return null;
			}
			if (fileUri.StartsWith("file://", true, null)) {
				fileUri = fileUri.Substring(6);
			}
			fileUri = SearchFile(fileUri, null);
			if (!string.IsNullOrEmpty(fileUri)) {
				var bytes = File.ReadAllBytes(fileUri);
				return new MemoryStream(bytes);
			}
			return null;
		}

		/// <summary>
		/// 获取绝对路径
		/// </summary>
		/// <param name="basePath">基本路径</param>
		/// <param name="path">相对路径</param>
		/// <param name="createdIfNotExists">如果目录不存在，是否自动创建</param>
		/// <returns>绝对路径</returns>
		public static string GetPhysicalPath(string basePath, string path, bool createdIfNotExists) {
			if (string.IsNullOrEmpty(basePath) || path == null) {
				return basePath;
			}
			if (basePath.StartsWith("~/")) {
				basePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, basePath.Substring(2));
			} else if (basePath.StartsWith("..") || basePath.StartsWith(".")) {
				basePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, basePath);
			}
			if (path.StartsWith("/") || path.StartsWith("\\")) {
				path = path.Substring(1);
			}
			path = Path.Combine(basePath, path);
			path = Path.GetFullPath(path);
			var fileName = Path.GetFileName(path);
			path = Path.GetDirectoryName(path);
			var directorySeparator = new string(Path.DirectorySeparatorChar, 1);//跨平台
			if(!path.EndsWith(directorySeparator)) {
				path += directorySeparator;
			}
			if (createdIfNotExists) {
				if (!Directory.Exists(path)) {
					Directory.CreateDirectory(path);
				}
			}
			if (!string.IsNullOrEmpty(fileName)) {
				path = Path.Combine(path, fileName);
			}
			return path;
		}

		/// <summary>
		/// 获取相对应用系统部署目录的绝对路径
		/// </summary>
		/// <param name="path">相对路径</param>
		/// <returns>绝对路径</returns>
		public static string GetFullPath(string path) {
			if (string.IsNullOrEmpty(path)) {
				return path;
			}
			if (Path.IsPathRooted(path)) {
				return path;
			}
			return Path.GetFullPath(AppDomain.CurrentDomain.BaseDirectory + Path.DirectorySeparatorChar + path);
		}

		#endregion

		#region Log

		private static readonly object lockObject = new object();

		/// <summary>
		/// 向指定文件添加文本信息
		/// </summary>
		/// <param name="logPath">所在路径</param>
		/// <param name="fileName">文本文件名（支持DateTime格式）</param>
		/// <param name="msgs">信息列表</param>
		public static void WriteLog(string logPath, string fileName, params object[] msgs) {
			fileName = string.Format("{0}{2}{1}", logPath, DateTime.Now.ToString(fileName), Path.DirectorySeparatorChar);
			WriteLog(fileName, msgs);
		}

		/// <summary>
		/// 向指定文件添加文本信息
		/// </summary>
		/// <param name="fileName">全路径文本文件名</param>
		/// <param name="msgs">信息列表</param>
		public static void WriteLog(string fileName, params object[] msgs) {
			//跨平台
			if (Environment.OSVersion.Platform == PlatformID.Unix || Environment.OSVersion.Platform == PlatformID.MacOSX) {
				fileName = fileName.Replace("\\", "/");
			}
			var file = new FileInfo(fileName);
			if (!file.Directory.Exists) {
				Directory.CreateDirectory(file.Directory.FullName);
			}
			if (msgs == null || msgs.Length == 0) {
				return;
			}
			var sb = new StringBuilder();
			foreach(var msg in msgs) {
				sb.Append(msg);
			}
			var message = sb.ToString();
			lock (lockObject) {
				File.AppendAllText(fileName, message);
			}
		}

		/// <summary>
		/// 向指定文件添加文本信息
		/// </summary>
		/// <param name="fileName">全路径文本文件名</param>
		/// <param name="message">文本信息</param>
		public static void WriteLog(string fileName, string message) {
			WriteLog(fileName, (object)message);
		}

		#endregion
	}
}

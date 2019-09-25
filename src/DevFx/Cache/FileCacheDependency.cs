/* Copyright(c) 2005-2011 R2@DevFx.NET, License(LGPL) */

using System;
using System.IO;

namespace DevFx.Cache
{
	/// <summary>
	/// 文件依赖方式的过期策略
	/// </summary>
	[Serializable]
	public class FileCacheDependency : CacheDependency
	{
		/// <summary>
		/// 构造函数
		/// </summary>
		/// <param name="fileName">需要监视的文件名（包含路径）</param>
		/// <param name="filters">监视方式</param>
		public FileCacheDependency(string fileName, NotifyFilters filters) {
			this.fileName = fileName;
			this.filters = filters;
			this.Init();
		}

		/// <summary>
		/// 构造函数
		/// </summary>
		/// <param name="fileName">需要监视的文件名（包含路径）</param>
		/// <remarks>
		/// 监视方式默认为文件的最后写入（修改）时间
		/// </remarks>
		public FileCacheDependency(string fileName) : this(fileName, NotifyFilters.LastWrite) {
		}

		private readonly string fileName;
		private FileSystemWatcher fileWatcher;
		private readonly NotifyFilters filters;
		private bool isExpired;

		private void Init() {
			var fileName = Path.GetFileName(this.fileName);
			var filePath = Path.GetDirectoryName(this.fileName);
			this.fileWatcher = new FileSystemWatcher(filePath, fileName) { NotifyFilter = this.filters };
			this.fileWatcher.Changed += this.FileChanged;
			this.fileWatcher.EnableRaisingEvents = true;
		}

		private void FileChanged(object sender, FileSystemEventArgs e) {
			this.fileWatcher.EnableRaisingEvents = false;
			this.isExpired = true;
			this.fileWatcher.Dispose();
			this.fileWatcher = null;
		}

		/// <summary>
		/// 是否已过期
		/// </summary>
		protected override bool IsExpired => this.isExpired;

		/// <summary>
		/// 重置缓存策略（相当于重新开始缓存）
		/// </summary>
		protected override void Reset() {
		}
	}
}

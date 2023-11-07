using System;
using System.IO;
using System.Threading.Tasks;
/****************************************************************
*  Copyright © (2022) www.fayelf.com All Rights Reserved.       *
*  Author : jacky                                               *
*  QQ : 7092734                                                 *
*  Email : jacky@fayelf.com                                     *
*  Site : www.fayelf.com                                        *
*  Create Time : 2022-04-15 11:28:53                            *
*  Version : v 1.0.0                                            *
*  CLR Version : 4.0.30319.42000                                *
*****************************************************************/
namespace XiaoFeng.IO
{
    /// <summary>
    /// 文件监控
    /// </summary>
    public class FileProvider : IDisposable
    {
        #region 构造器
        /// <summary>
        /// 无参构造器
        /// </summary>
        public FileProvider(string key)
        {
            this.Key = key;
        }
        #endregion

        #region 属性
        /// <summary>
        /// 释放值
        /// </summary>
        private bool disposedValue;
        /// <summary>
        /// 缓存Key
        /// </summary>
        public string Key { get; set; }
        /// <summary>
        /// 监控目录
        /// </summary>
        public string FolderPath { get; set; }
        /// <summary>
        /// 监控文件名
        /// </summary>
        public string FileName { get; set; }
        /// <summary>
        /// 监控器
        /// </summary>
        private FileSystemWatcher Watcher { get; set; }
        /// <summary>
        /// 是否运行
        /// </summary>
        public Boolean IsRun { get { return this.Watcher != null; } }
        #endregion

        #region 方法
        /// <summary>
        /// 监控文件
        /// </summary>
        /// <param name="filename">文件地址</param>
        /// <param name="callback">回调</param>
        /// <returns></returns>
        public async Task Watch(string filename, Action<WatcherChangeType, string, string> callback = null)
        {
            if (filename.IsNullOrEmpty()) return;
            var path = filename.GetBasePath();
            if (FileHelper.Exists(path))
            {
                var file = path.ToFileInfo();
                await Watch(file.Directory.FullName, file.Name, callback).ConfigureAwait(false);
            }
        }
        /// <summary>
        /// 监控文件
        /// </summary>
        /// <param name="folder">目录</param>
        /// <param name="filename">文件名</param>
        /// <param name="callback">回调</param>
        /// <returns></returns>
        public async Task Watch(string folder, string filename, Action<WatcherChangeType, string, string> callback = null)
        {
            if (folder.IsNullOrEmpty()) return;
            if (filename.IsNullOrEmpty()) filename = "*.*";
            this.FolderPath = folder;
            this.FileName = filename;
            this.Watcher = new FileSystemWatcher(folder, filename);
            if (filename == "*.*") this.Watcher.IncludeSubdirectories = true;
            this.Watcher.Changed += (sender, e) =>
            {
                callback?.Invoke(((int)e.ChangeType).ToCast<WatcherChangeType>(), e.FullPath, this.Key);
            };
            this.Watcher.Renamed += (sender, e) =>
            {
                if (this.Watcher.Filter != "*")
                    ((FileSystemWatcher)sender).EnableRaisingEvents = false;
                callback?.Invoke(((int)e.ChangeType).ToCast<WatcherChangeType>(), e.FullPath, this.Key);
            };
            this.Watcher.Deleted += (sender, e) =>
            {
                if (this.Watcher.Filter != "*")
                    ((FileSystemWatcher)sender).EnableRaisingEvents = false;
                callback?.Invoke(((int)e.ChangeType).ToCast<WatcherChangeType>(), e.FullPath, this.Key);
            };
            this.Watcher.Created += (sender, e) =>
            {
                if (this.Watcher.Filter != "*")
                    ((FileSystemWatcher)sender).EnableRaisingEvents = false;
                callback?.Invoke(((int)e.ChangeType).ToCast<WatcherChangeType>(), e.FullPath, this.Key);
            };
            this.Watcher.Error += (sender, e) =>
            {
                if (this.Watcher.Filter != "*")
                    ((FileSystemWatcher)sender).EnableRaisingEvents = false;
                callback?.Invoke(WatcherChangeType.Error, "", this.Key);
            };
            this.Watcher.EnableRaisingEvents = true;
            await Task.CompletedTask;
        }
        /// <summary>
        /// 关闭
        /// </summary>
        /// <returns></returns>
        public async Task CloseAsync()
        {
            if (this.Watcher != null)
                this.Watcher.EnableRaisingEvents = false;
            await Task.CompletedTask;
        }
        /// <summary>
        /// 释放
        /// </summary>
        /// <param name="disposing">值</param>
        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    this.CloseAsync().ConfigureAwait(false).GetAwaiter().GetResult();
                    if (this.Watcher != null)
                    {
                        this.Watcher.Dispose();
                        this.Watcher = null;
                    }
                }

                // TODO: 释放未托管的资源(未托管的对象)并重写终结器
                // TODO: 将大型字段设置为 null
                disposedValue = true;
            }
        }

        /// <summary>
        /// TODO: 仅当“Dispose(bool disposing)”拥有用于释放未托管资源的代码时才替代终结器
        /// </summary>
        ~FileProvider()
        {
            // 不要更改此代码。请将清理代码放入“Dispose(bool disposing)”方法中
            Dispose(disposing: false);
        }
        /// <summary>
        /// 释放
        /// </summary>
        public void Dispose()
        {
            // 不要更改此代码。请将清理代码放入“Dispose(bool disposing)”方法中
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
        #endregion
    }
}
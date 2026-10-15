using System;
using System.Collections.Concurrent;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using XiaoFeng.Config;
using XiaoFeng.Data;
using XiaoFeng.IO;

/****************************************************************
*  Copyright © (2023) www.eelf.cn All Rights Reserved.          *
*  Author : jacky                                               *
*  QQ : 7092734                                                 *
*  Email : jacky@eelf.cn                                        *
*  Site : www.eelf.cn                                           *
*  Create Time : 2023-09-25 18:44:27                            *
*  Version : v 1.0.0                                            *
*  CLR Version : 4.0.30319.42000                                *
*****************************************************************/
namespace XiaoFeng.Log
{
    /// <summary>
    /// 日志流
    /// </summary>
    public class LogStream
    {
        /// <summary>
        /// 设置配置
        /// </summary>
        /// <param name="filePath">文件路径</param>
        /// <param name="loggerConfig">日志配置</param>
        public LogStream(string filePath, LoggerConfig loggerConfig)
        {
            this.FilePath = filePath;
            this.LoggerConfig = loggerConfig;
            Init();
        }
        /// <summary>
        /// 日志配置
        /// </summary>
        public LoggerConfig LoggerConfig { get; set; }
        /// <summary>
        /// 文件路径
        /// </summary>
        public string FilePath { get; set; }
        /// <summary>
        /// 缓存大小
        /// </summary>
        public int BufferSize { get; set; } = 8192;
        /// <summary>
        /// 日志数据库
        /// </summary>
        public IDataHelper DataHelper { get; set; }
        /// <summary>
        /// 进程内文件锁集合
        /// </summary>
        private static readonly ConcurrentDictionary<string, object> FileLocks = new ConcurrentDictionary<string, object>();
        /// <summary>
        /// 数据库写入锁
        /// </summary>
        private static readonly object DbLock = new object();
        /// <summary>
        /// 跨进程文件锁集合
        /// </summary>
        private static readonly ConcurrentDictionary<string, Mutex> FileMutexes = new ConcurrentDictionary<string, Mutex>();
        /// <summary>
        /// 获取进程内文件锁
        /// </summary>
        /// <param name="filePath">文件路径</param>
        /// <returns></returns>
        private static object GetFileLock(string filePath) => FileLocks.GetOrAdd(GetMutexName(filePath), key => new object());
        /// <summary>
        /// 日志文件名
        /// </summary>
        public string FileName
        {
            get
            {
                if (FilePath.IsNullOrEmpty()) return "";
                return FilePath.GetFileNameWithoutExtension();
            }
        }
        /// <summary>
        /// 写日志
        /// </summary>
        /// <param name="logData">日志对象</param>
        /// <returns></returns>
        public Task WriteAsync(LogData logData)
        {
            this.Write(logData);
            return Task.CompletedTask;
        }
        /// <summary>
        /// 写日志
        /// </summary>
        /// <param name="logData">日志对象</param>
        private void Write(LogData logData)
        {
            var today = DateTime.Now.ToString("yyyy-MM-dd");
            if (this.FileName.StartsWith(today))
            {
                var fileInfo = new FileInfo(this.FilePath);
                if (fileInfo.Exists)
                {
                    if (this.LoggerConfig.FileLength > 0 && fileInfo.Length >= this.LoggerConfig.FileLength)
                    {
                        //分卷:循环滚动直到找到未超限的文件,避免只滚动一档导致分卷文件无限增长
                        this.FilePath = RollFileName(this.FilePath);
                        fileInfo = new FileInfo(this.FilePath);
                        while (this.LoggerConfig.FileLength > 0 && fileInfo.Exists && fileInfo.Length >= this.LoggerConfig.FileLength)
                        {
                            this.FilePath = RollFileName(this.FilePath);
                            fileInfo = new FileInfo(this.FilePath);
                        }
                        this.Init();
                    }
                }
                else
                    this.Init();
            }
            else
            {
                this.FilePath = this.FilePath.ReplacePattern(@"(\d+\-\d+\-\d+(_\d+)?)(\.log)$", today + ".log");
                this.ClearLog();
                this.Init();
            }
            if (this.LoggerConfig.StorageType.HasFlag(StorageType.File) && this.FilePath.IsNotNullOrEmpty())
            {
                //文件标志不匹配时只跳过文件写入,不能 return,否则会跳过后面的数据库写入
                if (this.LoggerConfig.FileFlags == 0 || this.LoggerConfig.FileFlags.HasFlag(logData.LogType))
                {
                    var mutex = GetFileMutex(this.FilePath);
                    var hasHandle = false;
                    try
                    {
                        try { hasHandle = mutex.WaitOne(); }
                        catch (AbandonedMutexException) { hasHandle = true; }
                        lock (GetFileLock(this.FilePath))
                        {
                            //跨进程安全:使用 FileMode.Append 原子追加到文件末尾,避免多进程并发写互相覆盖导致日志丢失
                            using (var fs = new FileStream(this.FilePath, FileMode.Append, FileAccess.Write, FileShare.ReadWrite, BufferSize))
                            {
                                var bytes = Encoding.UTF8.GetBytes(logData.ToString(LoggerConfig));
                                fs.Write(bytes, 0, bytes.Length);
                                fs.Flush();
                            }
                        }
                    }
                    finally
                    {
                        if (hasHandle) mutex.ReleaseMutex();
                    }
                }
            }
            if (this.DataHelper != null)
            {
                if (this.LoggerConfig.DataBaseFlags == 0 || this.LoggerConfig.DataBaseFlags.HasFlag(logData.LogType))
                {
                    lock (DbLock)
                    {
                        var sqlString = logData.ToSqlString(LoggerConfig);
                        if (sqlString.IsNotNullOrEmpty())
                            this.DataHelper.ExecuteNonQuery(sqlString);
                    }
                }
            }
        }
        /// <summary>
        /// 获取下一个分卷文件名 如 a.log -> a_1.log, a_1.log -> a_2.log
        /// </summary>
        /// <param name="filePath">当前文件路径</param>
        /// <returns></returns>
        private static string RollFileName(string filePath)
        {
            return filePath.ReplacePattern(@"(_(?<a>\d+))?\.log$", m =>
            {
                var val = m.Groups["a"]?.Value.ToCast<int>();
                return $"_{++val}.log";
            });
        }
        /// <summary>
        /// 初始化
        /// </summary>
        private void Init()
        {
            if (this.LoggerConfig.StorageType.HasFlag(StorageType.Database))
            {
                if (this.LoggerConfig.ConnectionStringKey.IsNotNullOrEmpty())
                {
                    this.DataHelper = new DataHelper(new ConnectionConfig(this.LoggerConfig.ConnectionStringKey));
                }
            }
            if (!this.LoggerConfig.StorageType.HasFlag(StorageType.File)) return;
            if (this.FilePath.IsNullOrEmpty()) return;
            FileHelper.CreateDirectory(this.FilePath.GetDirectoryName());
            //跨进程安全:确保文件存在,并在文件为空时写入文件头
            var mutex = GetFileMutex(this.FilePath);
            var hasHandle = false;
            try
            {
                try { hasHandle = mutex.WaitOne(); }
                catch (AbandonedMutexException) { hasHandle = true; }
                using (var fs = new FileStream(this.FilePath, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.ReadWrite, BufferSize))
                {
                    if (fs.Length < 10)
                    {
                        var head = Encoding.UTF8.GetBytes(GetFileHead);
                        fs.Write(head, 0, head.Length);
                        fs.Flush();
                    }
                }
            }
            finally
            {
                if (hasHandle) mutex.ReleaseMutex();
            }
        }
        /// <summary>
        /// 获取文件锁
        /// </summary>
        /// <param name="filePath">文件路径</param>
        /// <returns></returns>
        private static Mutex GetFileMutex(string filePath)
        {
            var name = GetMutexName(filePath);
            return FileMutexes.GetOrAdd(name, key => CreateMutex(key));
        }
        /// <summary>
        /// 创建跨进程互斥锁,优先使用全局命名空间(Global)以支持跨会话进程互斥,权限不足时降级为当前会话
        /// </summary>
        /// <param name="name">锁名称</param>
        /// <returns></returns>
        private static Mutex CreateMutex(string name)
        {
            try
            {
                return new Mutex(false, "Global\\" + name);
            }
            catch (UnauthorizedAccessException) { }
            catch (System.Security.SecurityException) { }
            return new Mutex(false, name);
        }
        /// <summary>
        /// 获取文件锁名称
        /// </summary>
        /// <param name="filePath">文件路径</param>
        /// <returns></returns>
        private static string GetMutexName(string filePath)
        {
            using (var md5 = System.Security.Cryptography.MD5.Create())
            {
                var hash = md5.ComputeHash(Encoding.UTF8.GetBytes(filePath.ToUpper()));
                return "XiaoFeng.LogStream." + BitConverter.ToString(hash).Replace("-", string.Empty);
            }
        }
        #region 获取文件头
        /// <summary>
        /// 获取文件头
        /// </summary>
        /// <returns></returns>
        private static string GetFileHead => @"{0}
                    App Log  v4.6
{0}
            Copyright: EELF.CN All rights Reserved.
            Author      : jacky
            QQ     : 7092734
            Email       : jacky@eelf.cn
            Create Time : {1}
{0}
".format(new string('=', 70), DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff"));
        #endregion

        #region 清理日志
        /// <summary>
        /// 清理日志
        /// </summary>
        private Task ClearLog()
        {
            if (LoggerConfig.StorageDays == 0) return Task.CompletedTask;
            return Task.Factory.StartNew(() =>
            {
                var days = LoggerConfig.StorageDays;
                if (days == 0) return;
                var filesPath = LoggerConfig.Path.GetBasePath();
                if (Directory.Exists(filesPath))
                {
                    var files = Directory.GetFiles(filesPath, "*.log", SearchOption.AllDirectories);
                    if (files.Length == 0) return;
                    files.Each(a =>
                    {
                        try
                        {
                            var file = a.GetMatch(@"(?<a>\d{4}-\d{2}-\d{2})(_\d+)?\.log$");
                            if (file.IsNullOrEmpty())
                            {
                                var f = new FileInfo(a);
                                if (DateTime.Now.Subtract(f.LastWriteTime).TotalDays>=days)
                                {
                                    f.Delete();
                                    LogHelper.Info("清理日志文件成功,文件路径:" + a);
                                }
                                return;
                            }
                            if (DateTime.Now.Subtract(file.ToDateTime()).TotalDays >= days)
                            {
                                File.Delete(a);
                                LogHelper.Info("清理日志文件成功,文件路径:" + a);
                            }
                        }
                        catch (Exception ex)
                        {
                            LogHelper.Error(ex, "删除日志文件失败,文件路径:" + a);
                        }
                    });
                }
            });
        }
        #endregion
    }
}
using System;
using System.IO;
using System.Text;
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
        /// 文件写流
        /// </summary>
        public StreamWriter Writer { get; set; }
        /// <summary>
        /// 日志数据库
        /// </summary>
        public IDataHelper DataHelper { get; set; }
        /// <summary>
        /// IO锁
        /// </summary>
        private static readonly object IOLock = new object();
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
        public async Task WriteAsync(LogData logData)
        {
            if (this.FileName.StartsWith(DateTime.Now.ToString("yyyy-MM-dd")))
            {
                if (this.Writer.BaseStream.Length > this.LoggerConfig.FileLength)
                {
                    this.FilePath = this.FilePath.ReplacePattern(@"(_(?<a>\d+))?\.log$", m =>
                    {
                        var val = m.Groups["a"]?.Value.ToCast<int>();
                        return $"_{++val}.log";
                    });
                    this.Init();
                }
            }
            else
            {
                this.FilePath = this.FilePath.ReplacePattern(@"(\d+\-\d+\-\d+(_\d+)?)(\.log)$", DateTime.Now.ToString("yyyy-MM-dd") + ".log");
                this.ClearLog().ConfigureAwait(false).GetAwaiter();
                this.Init();
            }
            if (this.Writer != null && this.Writer.BaseStream.CanWrite)
            {
                if (!this.LoggerConfig.FileFlags.HasFlag(logData.LogType) && this.LoggerConfig.FileFlags != 0) return;
                lock (IOLock)
                {
                    this.Writer.WriteAsync(logData.ToString(LoggerConfig)).ConfigureAwait(false).GetAwaiter().GetResult();
                }
            }
            if (this.DataHelper != null)
            {
                if (!this.LoggerConfig.DataBaseFlags.HasFlag(logData.LogType) && this.LoggerConfig.DataBaseFlags != 0) return;
                lock (IOLock)
                {
                    this.DataHelper.ExecuteNonQuery(logData.ToSqlString(LoggerConfig));
                }
            }
            await Task.CompletedTask;
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
            lock (IOLock)
            {
                if (this.Writer != null)
                {
                    this.Writer.FlushAsync().ConfigureAwait(false).GetAwaiter().GetResult();
                    this.Writer.Close();
                    this.Writer.Dispose();
                }
                this.Writer = new StreamWriter(new FileStream(this.FilePath, FileMode.OpenOrCreate, FileAccess.Write, FileShare.ReadWrite, 8192, FileOptions.Asynchronous), Encoding.UTF8)
                {
                    AutoFlush = true
                };
                if (this.Writer.BaseStream.Length < 10)
                {
                    this.Writer.WriteAsync(GetFileHead).ConfigureAwait(false).GetAwaiter().GetResult();
                }
                this.Writer.BaseStream.Seek(0, SeekOrigin.End);
            }
        }
        #region 获取文件头
        /// <summary>
        /// 获取文件头
        /// </summary>
        /// <returns></returns>
        private static string GetFileHead => @"{0}
                    App Log  v4.5
{0}
            Company Name: FayElf(www.eelf.cn)
            Author      : jacky
            QQ Code     : 7092734
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
                            if (file.IsNullOrEmpty()) return;
                            if (DateTime.Now.Subtract(file.ToDateTime()).TotalDays >= days)
                            {
                                File.Delete(a);
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
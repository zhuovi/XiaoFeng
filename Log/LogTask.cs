using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;
using XiaoFeng.Config;
using XiaoFeng.IO;
using XiaoFeng.Threading;

/****************************************************************
*  Copyright © (2023) www.eelf.cn All Rights Reserved.          *
*  Author : jacky                                               *
*  QQ : 7092734                                                 *
*  Email : jacky@eelf.cn                                        *
*  Site : www.eelf.cn                                           *
*  Create Time : 2023-09-25 13:49:01                            *
*  Version : v 1.0.0                                            *
*  CLR Version : 4.0.30319.42000                                *
*****************************************************************/
namespace XiaoFeng.Log
{
    /// <summary>
    /// 新日志队列
    /// </summary>
    public class LogTask : TaskServiceQueue<LogData>
    {
        #region 构造器
        /// <summary>
        /// 无参构造器
        /// </summary>
        public LogTask()
        {
            this.LogWriter = new ConcurrentDictionary<string, LogStream>();
            this.LoggerConfig = LoggerConfig.Current;
        }

        #endregion

        #region 属性
        /// <summary>
        /// 日志配置
        /// </summary>
        public LoggerConfig LoggerConfig { get; set; }
        /// <summary>
        /// 日志集合
        /// </summary>
        public ConcurrentDictionary<string, LogStream> LogWriter { get; set; }
        #endregion

        #region 方法
        ///<inheritdoc/>
        public override Task AddWorkItem(LogData t)
        {
            if ((int)t.LogType < (int)this.LoggerConfig.LogLevel) return Task.CompletedTask;
            if (this.LoggerConfig.StorageType.HasFlag(StorageType.Console))
            {
                t.ConsoleOutput(this.LoggerConfig);
            }
            if (!this.LoggerConfig.OpenLog) return Task.CompletedTask;

            if ((this.LoggerConfig.StorageType.HasFlag(StorageType.File) && this.LoggerConfig.FileFlags.HasFlag(t.LogType)) || (this.LoggerConfig.StorageType.HasFlag(StorageType.Database) && this.LoggerConfig.DataBaseFlags.HasFlag(t.LogType)))
                return base.AddWorkItem(t);

            return Task.CompletedTask;
        }
        ///<inheritdoc/>
        public override async Task ExecuteAsync(LogData workItem, CancellationToken cancellationToken)
        {
            if (base.Count >= 65535) base.Clear().ConfigureAwait(false).GetAwaiter();
            var logType = workItem.FilePath.IsNullOrEmpty() ? workItem.LogType.ToString() : workItem.FilePath;
            if (LogWriter.TryGetValue(logType, out var writer))
            {
                await writer.WriteAsync(workItem).ConfigureAwait(false);
            }
            else
            {
                var path = workItem.FilePath.IsNullOrEmpty() ? GetLogPath(workItem.LogType) : workItem.FilePath;
                var Writer = new LogStream(path, LoggerConfig);
                LogWriter.TryAdd(logType, Writer);
                await this.ExecuteAsync(workItem, cancellationToken).ConfigureAwait(false);
            }
            await Task.CompletedTask;
        }
        /// <summary>
        /// 获取日志文件
        /// </summary>
        /// <param name="logType">日志类型</param>
        /// <returns></returns>
        private string GetLogPath(LogType logType) => FileHelper.Combine(FileHelper.CurrentDirectory, LoggerConfig.Path, logType.ToString(), DateTime.Now.ToString("yyyy-MM-dd") + ".log");
        #endregion
    }
}
using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using XiaoFeng.Config;
using XiaoFeng.Data;
using XiaoFeng.IO;
using XiaoFeng.Threading;
/****************************************************************
*  Copyright © (2021) www.fayelf.com All Rights Reserved.      *
*  Author : jacky                                              *
*  QQ : 7092734                                                *
*  Email : jacky@fayelf.com                                    *
*  Site : www.fayelf.com                                       *
*  Create Time : 2021-05-28 16:05:45                           *
*  Version : v 2.0.0                                           *
*  CLR Version : 4.0.30319.42000                               *
****************************************************************/
namespace XiaoFeng.Log
{
    /// <summary>
    /// 日志操作类
    /// </summary>
    public class Logger : BaseLog
    {
        #region 构造器

        #endregion

        #region 属性
        /// <summary>
        /// 日志文件路径集合 按日志类型(含Record)分类缓存
        /// </summary>
        private ConcurrentDictionary<string, string> LogFilePaths { get; } = new ConcurrentDictionary<string, string>();
        #endregion

        #region 方法

        #region 运行
        ///<inheritdoc/>
        public override void Run(LogData log)
        {
            if (log.LogType < this.LogLevel) return;
            var sbr = new StringBuilder();
            /*控制台*/
            if (this.StorageType.HasFlag(StorageType.Console) && this.ConsoleFlags.HasFlag(log.LogType))
            {
                log.ConsoleOutput(this.Config);
            }

            if (!this.Config.OpenLog) return;
            /*文件*/
            if (this.StorageType.HasFlag(StorageType.File) || log.LogType == LogType.Error)
            {
                var date = DateTime.Now.ToString("yyyy-MM-dd");
                var category = log.IsRecord ? "Record" : log.LogType.ToString();
                /*缓存路径为空或文件日期已过期(跨天)时重新计算,避免每次写日志重复扫描目录并触发日志清理*/
                if (!this.LogFilePaths.TryGetValue(category, out var logFilePath) || !logFilePath.GetFileName().StartsWith(date))
                {
                    var logPath = this.LogPath;
                    if (logPath.IsNullOrEmpty())
                        logPath = "Log";
                    logPath += FileHelper.AltDirectorySeparatorChar + category;
                    logPath = logPath.GetBasePath();
                    FileHelper.Create(logPath, FileAttribute.Directory);
                    string FilePath = "";
                    var file = logPath.ToDirectoryInfo().GetFiles(date + "*", SearchOption.TopDirectoryOnly).OrderByDescending(a => a.Name.Contains("_") ? a.Name.GetMatch(@"_(?<a>\d+)\.log").ToCast<int>() : 0).FirstOrDefault();

                    if (file == null)
                    {
                        FilePath = (logPath + FileHelper.AltDirectorySeparatorChar + date + ".log").GetBasePath();
                    }
                    else
                    {
                        if (this.Config.FileLength > 0 && file.Length >= this.Config.FileLength)
                        {
                            FilePath = file.FullName.ReplacePattern(@"(_(?<a>\d+))?\.log$", m =>
                            {
                                var val = m.Groups["a"]?.Value.ToCast<int>();
                                return $"_{++val}.log";
                            });
                        }
                        else
                            FilePath = file.FullName;
                    }
                    logFilePath = FilePath;
                    this.LogFilePaths[category] = logFilePath;
                }
                new LogStream(logFilePath, this.Config).WriteAsync(log).ForgetTaskSafe();
            }
            /*数据库*/
            if (this.StorageType.HasFlag(StorageType.Database) && log.LogType == LogType.Error)
            {
                if (this.ConnectionConfig.ConnectionString.IsNotNullOrEmpty() && this.Config.InsertSql.IsNotNullOrEmpty())
                {
                    var data = new DataHelper(this.ConnectionConfig);

                    data.ExecuteNonQuery(log.ToSqlString(this.Config));
                }
            }
        }
        #endregion

        #region 释放对象
        /// <summary>
        /// 释放对象
        /// </summary>
        /// <param name="disposing">状态</param>
        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
        }
        #endregion

        #endregion
    }
}
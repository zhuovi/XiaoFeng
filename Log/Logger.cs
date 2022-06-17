using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using XiaoFeng.Config;
using XiaoFeng.Data;
using XiaoFeng.IO;
/****************************************************************
*  Copyright © (2021) www.fayelf.com All Rights Reserved.      *
*  Author : jacky                                              *
*  QQ : 7092734                                                *
*  Email : jacky@fayelf.com                                    *
*  Site : www.fayelf.com                                       *
*  Create Time : 2021-05-28 16:05:45                           *
*  Version : v 1.0.0                                           *
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
        /// 文件锁
        /// </summary>
        private static readonly object FileLock = new object();
        /// <summary>
        /// 计时器
        /// </summary>
        private static int Count = 0;
        #endregion

        #region 方法

        #region 运行
        ///<inheritdoc/>
        public override void Run(LogData log)
        {
            if (log.LogType < this.LogLevel) return;
            /*控制台*/
            if (this.StorageType.HasFlag(StorageType.Console) && this.ConsoleFlags.HasFlag(log.LogType))
            {
                lock (FileLock)
                {
                    var ForegroundColor = (int)Console.ForegroundColor;
                    var BackgroundColor = (int)Console.BackgroundColor;
                    var foreColor = Console.ForegroundColor;
                    var backColor = Console.BackgroundColor;
                    switch (log.LogType)
                    {
                        case LogType.Error:
                            foreColor = ConsoleColor.Red;
                            break;
                        case LogType.Info:
                            foreColor = ConsoleColor.Green;
                            break;
                        case LogType.SQL:
                            foreColor = ConsoleColor.DarkMagenta;
                            break;
                        case LogType.Debug:
                            foreColor = ConsoleColor.DarkYellow;
                            break;
                        case LogType.Warn:
                            foreColor = ConsoleColor.Magenta;
                            break;
                        case LogType.Trace:
                            foreColor = ConsoleColor.Blue;
                            break;
                        case LogType.Task:
                            foreColor = ConsoleColor.Cyan;
                            break;
                    }
                    Console.ForegroundColor = foreColor;
                    Console.BackgroundColor = backColor;
                    if (log.IsRecord)
                    {
                        if (log.Message.IsNotNullOrEmpty())
                        {
                            Console.WriteLine($"{log.LogType.GetValue(this.MessageType)}".PadRight(this.MessageType == EnumValueType.Description ? 2 : 5, ' ') + (this.Config.Fields.Contains("AddTime") ? DateTime.Now.ToString(": yyyy-MM-dd HH:mm:ss.fff") : ""));
                            Console.ResetColor();
                            Console.WriteLine(log.Message);
                        }
                    }
                    else
                    {
                        if (this.Config.Fields.Contains("LogType"))
                        {
                            Console.WriteLine("".PadLeft(70, '='));
                            Console.WriteLine($"{log.LogType.GetValue(this.MessageType)}".PadRight(this.MessageType == EnumValueType.Description ? 2 : 5, ' ') + (this.Config.Fields.Contains("AddTime") ? DateTime.Now.ToString(": yyyy-MM-dd HH:mm:ss.fff") : ""));
                            Console.WriteLine("".PadLeft(70, '='));
                        }
                        Console.ResetColor();
                        if (log.DataSource.IsNotNullOrEmpty() && (this.Config.Fields.Length==0 || (this.Config.Fields.Length>0 && this.Config.Fields.Contains("DataSource")))) Console.WriteLine("数 据 源: " + log.DataSource);
                        if (log.FunctionName.IsNotNullOrEmpty() && (this.Config.Fields.Length==0 || (this.Config.Fields.Length > 0 && this.Config.Fields.Contains("FunctionName")))) Console.WriteLine("方 法 名: " + log.ClassName + "." + log.FunctionName);
                        if (log.Message.IsNotNullOrEmpty() && (this.Config.Fields.Length == 0 || (this.Config.Fields.Length > 0 && this.Config.Fields.Contains("Message")))) Console.WriteLine((log.LogType == LogType.Warn ? "" : "日志信息: ") + log.Message);
                        if (log.StackTrace.IsNotNullOrEmpty() && (this.Config.Fields.Length == 0 || (this.Config.Fields.Length > 0 && this.Config.Fields.Contains("StackTrace")))) Console.WriteLine("日志堆栈: " + log.StackTrace);
                        if ((LogType.Error | LogType.Trace).HasFlag(log.LogType) && log.Tracking.IsNotNullOrEmpty() && (this.Config.Fields.Length == 0 || (this.Config.Fields.Length > 0 && this.Config.Fields.Contains("Tracking"))))
                        {
                            Console.WriteLine("堆栈跟踪".PadLeft(33, '*').PadRight(66, '*'));
                            var trace = log.Tracking ?? new StackTrace();
                            trace.GetFrames().Each(frame =>
                            {
                                System.Reflection.MethodBase method = frame?.GetMethod();
                                if (method == null || method.DeclaringType?.FullName.IndexOf("LogHelper") > -1) return;
                                var _ = "{0}.{1}({2});".format(method.DeclaringType?.FullName, method.Name, method.GetParameters().Join(","));
                                Console.WriteLine(_);
                                return;
                            });
                            Console.WriteLine("".PadLeft(70, '*'));
                        }
                    }
                }
            }
            if (!this.Config.OpenLog) return;
            /*文件*/
            if (this.StorageType.HasFlag(StorageType.File) || log.LogType == LogType.Error)
            {
                StreamWriter sw = null;
                try
                {
                    Monitor.Enter(FileLock);
                    var logPath = this.LogPath;
                    if (logPath.IsNullOrEmpty())
                        logPath = "Log";
                    logPath += FileHelper.AltDirectorySeparatorChar + (log.IsRecord ? "Record" : log.LogType.ToString());
                    logPath = logPath.GetBasePath();

                    FileHelper.Create(logPath, FileAttribute.Directory);

                    string FilePath = (logPath + FileHelper.AltDirectorySeparatorChar + DateTime.Now.ToString("yyyy-MM-dd") + ".log").GetBasePath();
                    int i = 1;
                    while (File.Exists(FilePath))
                    {
                        string _FilePath = FilePath.ReplacePattern(@"(_\d+)?.log$", "_" + i.ToString() + ".log");
                        if (!File.Exists(_FilePath))
                        {
                            var bytes = FileHelper.OpenBytes(FilePath);
                            if (bytes.Length > this.Config.FileLength)
                                FilePath = _FilePath;
                            break;
                        }
                        else
                        {
                            FilePath = _FilePath;
                            i++;
                        }
                    }
                    ExistsFile(FilePath);
                    sw = File.AppendText(FilePath);
                    if (log.IsRecord)
                    {
                        if (log.Message.IsNotNullOrEmpty())
                        {
                            sw.WriteLine($"{log.LogType.GetValue(this.MessageType)}".PadRight(this.MessageType == EnumValueType.Description ? 2 : 5, ' ') + (this.Config.Fields.Contains("AddTime") ? DateTime.Now.ToString(": yyyy-MM-dd HH:mm:ss.fff") : ""));
                            sw.WriteLine(log.Message);
                        }
                    }
                    else
                    {
                        if (this.Config.Fields.Contains("LogType"))
                        {
                            sw.WriteLine("".PadLeft(70, '='));
                            sw.WriteLine($"{log.LogType.GetValue(this.MessageType)}".PadRight(this.MessageType == EnumValueType.Description ? 2 : 5, ' ') + (this.Config.Fields.Contains("AddTime") ? DateTime.Now.ToString(": yyyy-MM-dd HH:mm:ss.fff") : ""));
                            sw.WriteLine("".PadLeft(70, '='));
                        }
                        if (log.DataSource.IsNotNullOrEmpty() && (this.Config.Fields.Length == 0 || (this.Config.Fields.Length > 0 && this.Config.Fields.Contains("StackTrace")))) sw.WriteLine("数 据 源: " + log.DataSource);
                        if (log.FunctionName.IsNotNullOrEmpty() && (this.Config.Fields.Length == 0 || (this.Config.Fields.Length > 0 && this.Config.Fields.Contains("FunctionName")))) sw.WriteLine("方 法 名: " + log.ClassName + "." + log.FunctionName);
                        if (log.Message.IsNotNullOrEmpty() && (this.Config.Fields.Length == 0 || (this.Config.Fields.Length > 0 && this.Config.Fields.Contains("Message")))) sw.WriteLine("日志信息: " + log.Message);
                        if (log.StackTrace.IsNotNullOrEmpty() && (this.Config.Fields.Length == 0 || (this.Config.Fields.Length > 0 && this.Config.Fields.Contains("StackTrace")))) sw.WriteLine("日志堆栈: " + log.StackTrace);
                        if ((LogType.Error | LogType.Trace).HasFlag(log.LogType) && log.Tracking.IsNotNullOrEmpty() && (this.Config.Fields.Length == 0 || (this.Config.Fields.Length > 0 && this.Config.Fields.Contains("Tracking"))))
                        {
                            sw.WriteLine("堆栈跟踪".PadLeft(33, '*').PadRight(66, '*'));
                            var trace = log.Tracking ?? new StackTrace();
                            trace.GetFrames().Each(frame =>
                            {
                                System.Reflection.MethodBase method = frame?.GetMethod();
                                if (method == null || method.DeclaringType?.FullName.IndexOf("LogHelper") > -1) return;
                                sw.WriteLine("{0}.{1}({2});".format(method.DeclaringType?.FullName, method.Name, method.GetParameters().Join(",")));
                                return;
                            });
                            sw.WriteLine("".PadLeft(70, '*'));
                        }
                    }
                    sw.Flush();
                    sw.Close();
                    sw.Dispose();
                    Monitor.Exit(FileLock);
                }
                catch
                {
                    if (sw != null) { sw.Close(); sw.Dispose(); }
                    if (Interlocked.Increment(ref Count) == 3)
                    {
                        Count = 0;
                        return;
                    }
                    Run(log);
                }
            }
            /*数据库*/
            if (this.StorageType.HasFlag(StorageType.Database))
            {
                if (this.ConnectionConfig.ConnectionString.IsNotNullOrEmpty() && this.Config.InsertSql.IsNotNullOrEmpty())
                {
                    var Tracking = "堆栈跟踪".PadLeft(33, '*').PadRight(66, '*');
                    var trace = log.Tracking ?? new StackTrace();
                    trace.GetFrames().Each(frame =>
                    {
                        System.Reflection.MethodBase method = frame?.GetMethod();
                        if (method == null || method.DeclaringType?.FullName.IndexOf("LogHelper") > -1) return;
                        var _ = "{0}.{1}({2});".format(method.DeclaringType?.FullName, method.Name, method.GetParameters().Join(","));
                        Tracking += _;
                        return;
                    });
                    Tracking += "".PadLeft(70, '*');
                    var data = new DataHelper(this.ConnectionConfig);
                    var sql = this.Config.InsertSql.ReplacePattern(@"\{(?<a>[a-z]+)\}", m =>
                    {
                        var name = m.Groups["a"].Value;
                        return name.EqualsIgnoreCase("Tracking") ? Tracking : log[name].ToString();
                    });
                    data.ExecuteNonQuery(sql);
                }
            }
        }
        #endregion

        #region 文件是否存在
        /// <summary>
        /// 文件是否存在
        /// </summary>
        /// <param name="FilePath">文件路径</param>
        private void ExistsFile(string FilePath)
        {
            if (!FileHelper.Exists(FilePath, FileAttribute.File))
            {
                FileHelper.WriteText(FilePath, @"{0}
                    应用日志文件  v4.3
{0}
            Company : 魔法精灵(www.fayelf.com)
            Author : jacky
              QQ : 7092734
           Email : jacky@fayelf.com
        Create Time : {1}
{0}
".format(new string('=', 70), DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff") + Environment.NewLine));

                if (FilePath.IsMatch(@"\d{4}-\d{2}-\d{2}\.log$")) ClearLog();
            }
        }
        #endregion

        #region 清理日志
        /// <summary>
        /// 清理日志
        /// </summary>
        private void ClearLog()
        {
            System.Threading.Tasks.Task.Factory.StartNew(() =>
            {
                var Config = LoggerConfig.Current;
                var days = Config.StorageDays;
                if (days == 0) return;
                string FileName = "*.log";
                var filesPath0 = Config.Path.GetBasePath();
                string[] files0;
                if (Directory.Exists(filesPath0))
                {
                    files0 = Directory.GetFiles(filesPath0, FileName, SearchOption.AllDirectories);
                }
                else files0 = Array.Empty<String>();
                
                var files = new string[files0.Length];
                if (files.Length == 0) return;
                if (files0.Length > 0) files0.CopyTo(files, 0);
                files.Each(a =>
                {
                    try
                    {
                        var file = a.GetMatch(@"\d{4}-\d{2}-\d{2}");
                        if (file.IsNullOrEmpty()) return;
                        if (DateTime.Now.Subtract(file.ToDateTime()).TotalDays >= days)
                        {
                            File.Delete(a);
                        }
                    }
                    catch (Exception ex)
                    {
                        Error(ex, "删除日志文件失败,文件路径:" + a);
                    }
                });
            });
        }
        #endregion

        #endregion
    }
}
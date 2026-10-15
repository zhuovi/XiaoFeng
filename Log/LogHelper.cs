using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using XiaoFeng.Config;
using XiaoFeng.Log;
using XiaoFeng.Threading;
/****************************************************************
*  Copyright © (2017) www.fayelf.com All Rights Reserved.       *
*  Author : jacky                                               *
*  QQ : 7092734                                                 *
*  Email : jacky@fayelf.com                                     *
*  Site : www.fayelf.com                                        *
*  Create Time : 2017-10-25 11:59:42                            *
*  Version : v 1.0.0                                            *
*  CLR Version : 4.0.30319.42000                                *
*****************************************************************/
namespace XiaoFeng
{
    /// <summary>
    /// 日志操作类
    /// Version : 2.5
    /// Update Date : 2019-09-20
    /// </summary>
    public static class LogHelper
    {
        #region 构造器
        /// <summary>
        /// 构造器
        /// </summary>
        static LogHelper()
        {
            Log = LogFactory.Create(typeof(Logger), "LogTask");
        }
        #endregion

        #region 属性
        /// <summary>
        /// 日志对象
        /// </summary>
        private static ILog Log { get; set; }
        /// <summary>
        /// 日志队列
        /// </summary>
        private static ITaskServiceQueue<LogData> LogTaskQueue = new LogTask();
        /// <summary>
        /// 静态锁
        /// </summary>
        private static object lockObject = new object();
        #endregion

        #region 方法

        #region 记录日志
        /// <summary>
        /// 记录日志
        /// </summary>
        /// <param name="logData">日志对象</param>
        public static void WriteLog(LogData logData)
        {
            LogTaskQueue.AddWorkItem(logData);
        }
        /// <summary>
        /// 记录日志
        /// </summary>
        /// <param name="logType">日志类型</param>
        /// <param name="DataSource">日志源</param>
        /// <param name="ClassName">日志类名</param>
        /// <param name="FunctionName">方法名</param>
        /// <param name="Message">日志信息</param>
        public static void WriteLog(LogType logType, string DataSource, string ClassName, string FunctionName, string Message)
        {
            WriteLog(new LogData
            {
                LogType = logType,
                DataSource = DataSource,
                ClassName = ClassName,
                FunctionName = FunctionName,
                Message = Message
            });
        }
        /// <summary>
        /// 记录日志
        /// </summary>
        /// <param name="logType">日志类型</param>
        /// <param name="Message">日志信息</param>
        public static void WriteLog(LogType logType, string Message)
        {
            WriteLog(new LogData
            {
                LogType = logType,
                Message = Message
            });
        }
        /// <summary>
        /// 记录日志
        /// </summary>
        /// <param name="Message">日志信息</param>
        public static void WriteLog(string Message)
        {
            WriteLog(new LogData
            {
                LogType = LogType.Info,
                Message = Message
            });
        }
        /// <summary>
        /// 记录日志
        /// </summary>
        /// <param name="ex">错误信息</param>
        /// <param name="Message">信息</param>
        public static void WriteLog(Exception ex, string Message = "")
        {
            ex = ex ?? new Exception();
            WriteLog(new LogData
            {
                LogType = LogType.Error,
                Message = ex?.Message + (Message.IsNullOrEmpty() ? "" : "[自定义信息:' {0} ']".format(Message)),
                DataSource = ex?.Source,
                ClassName = ex?.TargetSite == null ? "" : ex?.TargetSite?.DeclaringType?.Name,
                FunctionName = ex?.TargetSite == null ? "" : ex?.TargetSite?.Name,
                StackTrace = ex?.StackTrace ?? "",
                Tracking = new StackTrace()
            });
        }
        /// <summary>
        /// 记录日志
        /// </summary>
        /// <param name="ex">错误信息</param>
        /// <param name="Message">信息</param>
        public static void Error(Exception ex, string Message = "") => WriteLog(ex, Message);
        /// <summary>
        /// 记录日志
        /// </summary>
        /// <param name="Message">信息</param>
        public static void Info(string Message) => WriteLog(LogType.Info, Message);
        /// <summary>
        /// 记录日志
        /// </summary>
        /// <param name="Message">信息</param>
        public static void Debug(string Message) => WriteLog(LogType.Debug, Message);
        /// <summary>
        /// 记录日志
        /// </summary>
        /// <param name="Message">信息</param>
        public static void SQL(string Message) => WriteLog(LogType.SQL, Message);
        /// <summary>
        /// 记录日志
        /// </summary>
        /// <param name="Message">信息</param>
        public static void Warn(string Message) => WriteLog(LogType.Warn, Message);
        /// <summary>
        /// 任务日志
        /// </summary>
        /// <param name="Message">信息</param>
        public static void Task(string Message) => WriteLog(LogType.Task, Message);
        /// <summary>
        /// 跟踪日志
        /// </summary>
        /// <param name="Message">信息</param>
        public static void Trace(string Message) => WriteLog(LogType.Trace, Message);
        /// <summary>
        /// 记录日志
        /// </summary>
        /// <param name="Message">信息</param>
        /// <param name="logType">日志类型</param>
        public static void Record(string Message, LogType logType = LogType.Info) => WriteLog(new LogData
        {
            IsRecord = true,
            Message = Message,
            LogType = logType
        });
        #endregion

        #region 调试日志
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
        /// 跨进程文件锁集合
        /// </summary>
        private static readonly ConcurrentDictionary<string, Mutex> FileMutexes = new ConcurrentDictionary<string, Mutex>();
        /// <summary>
        /// 进程内文件锁集合
        /// </summary>
        private static readonly ConcurrentDictionary<string, object> FileLocks = new ConcurrentDictionary<string, object>();
        /// <summary>
        /// 获取进程内文件锁
        /// </summary>
        /// <param name="filePath">文件路径</param>
        /// <returns></returns>
        private static object GetFileLock(string filePath) => FileLocks.GetOrAdd(GetMutexName(filePath), key => new object());
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
        /// <summary>
        /// 写调试日志
        /// </summary>
        /// <param name="message">日志信息</param>
        /// <param name="path">日志路径</param>
        public static void DebugLogger(string message, string path = "debug.log")
        {
            var entry = System.Reflection.Assembly.GetEntryAssembly();
            var debuggable = entry.GetCustomAttributes(typeof(DebuggableAttribute), false).FirstOrDefault();
            if (debuggable != null)
            {
                var debug = debuggable as DebuggableAttribute;
                if (!debug.IsJITTrackingEnabled) return;
            }
            if (path.IsNullOrEmpty()) path = "debug.log";
            path = Path.GetFullPath("debug.log");
            var msg = $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss.fffffff}] - {message}";
            var mutex = GetFileMutex(path);
            var hasHandle = false;
            try
            {
                try { hasHandle = mutex.WaitOne(); }
                catch (AbandonedMutexException) { hasHandle = true; }
                lock (GetFileLock(path))
                {
                    //跨进程安全:使用 FileMode.Append 原子追加到文件末尾,避免多进程并发写互相覆盖导致日志丢失
                    using (var fs = new FileStream(path, FileMode.Append, FileAccess.Write, FileShare.ReadWrite, 8192))
                    {
                        var bytes = Encoding.UTF8.GetBytes(msg);
                        fs.Write(bytes, 0, bytes.Length);
                        fs.Flush();
                    }
                }
            }
            finally
            {
                if (hasHandle) mutex.ReleaseMutex();
            }
            lock (lockObject)
            {
                var oColor = Console.ForegroundColor;
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine(msg);
                Console.ForegroundColor = oColor;
            }
        }
        #endregion

        #endregion
    }
}
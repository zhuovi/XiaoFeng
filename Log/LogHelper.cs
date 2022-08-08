using System;
using System.Collections.Generic;
using System.Web;
using System.Threading;
using System.IO;
using System.Text.RegularExpressions;
using System.Diagnostics;
using System.Text;
using System.ComponentModel;
using XiaoFeng.Config;
using XiaoFeng.IO;
using XiaoFeng.Log;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using XiaoFeng.Threading;

namespace XiaoFeng
{
    /// <summary>
    /// 日志操作类
    /// Email : jacky@fayelf.com
    /// QQ : 7092734
    /// Site : www.fayelf.com
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
            /*初始化数据*/
            //ThreadPool.SetMaxThreads(30, 30);
            //ThreadPool.SetMinThreads(1, 1);

            //var config = LoggerConfig.Current;
            /*是否写日志*/
            //IsWriteLog = config.OpenLog;
            //if (!IsWriteLog) return;
            /*路径*/
            //if (LogPath == "")
            {
                //LogPath = config.Path;
                //if (LogPath.IsNullOrEmpty()) LogPath = "Log";
                //LogPath = LogPath.GetBasePath();
            }
            //FileHelper.Create(LogPath, FileAttribute.Directory);
        }
        #endregion

        #region 属性
        ///// <summary>
        ///// 日志保存路径
        ///// </summary>
        //private static string LogPath = "";
        /// <summary>
        /// 文件锁
        /// </summary>
        private static readonly object FileLock = new object();
        ///// <summary>
        ///// 是否写日志
        ///// </summary>
        //private static Boolean IsWriteLog = true;
        ///// <summary>
        ///// 计时器
        ///// </summary>
        //private static int Count = 0;
        /// <summary>
        /// 日志对象
        /// </summary>
        private static ILog Log { get; set; }
        /// <summary>
        /// 日志队列
        /// </summary>
        //private static IBackgroundTaskQueue LogQueue = new BackgroundTaskQueue("LogTaskQueue");
        private static ITaskServiceQueue<LogData> LogTaskQueue = new LogTaskQueue();
        #endregion

        #region 方法

        #region 记录日志
        /// <summary>
        /// 记录日志
        /// </summary>
        /// <param name="logData">日志对象</param>
        public static void WriteLog(LogData logData)
        {
            //lock (FileLock)
            {
                //if (LogQueue == null) LogQueue = new BackgroundTaskQueue("LogTaskQueue");
                //if (Log == null) Log = LogFactory.Create(typeof(Logger), "LogTaskQueue");
            }
            //LogQueue.AddWorkItem(() =>
            //{
            //Log.Write(logData);
            //});
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
        /// 记录日志
        /// </summary>
        /// <param name="Message">信息</param>
        /// <param name="logType">日志类型</param>
        public static void Record(string Message,LogType logType =  LogType.Info) => WriteLog(new LogData
        {
            IsRecord = true,
            Message = Message,
            LogType = logType
        });
        #endregion

        #region 设置日志目录
        /// <summary>
        /// 设置日志目录
        /// </summary>
        /// <param name="path">目录</param>
        public static void SetLogPath(string path)
        {
            if (Log == null)
                Log = LogFactory.Create(typeof(Logger), "LogTask");
            Log.LogPath = path;
        }
        #endregion

        #endregion
    }
}
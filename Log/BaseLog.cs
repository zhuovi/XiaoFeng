using System;
using System.Diagnostics;
using XiaoFeng.Config;
using XiaoFeng.Data;
/****************************************************************
 *  Copyright © (2021) www.fayelf.com All Rights Reserved.      *
 *  Author : jacky                                              *
 *  QQ : 7092734                                                *
 *  Email : jacky@fayelf.com                                    *
 *  Site : www.fayelf.com                                       *
 *  Create Time : 2021/5/7 11:08:06                             *
 *  Version : v 1.0.0                                           *
 *  CLR Version : 4.0.30319.42000                               *
 ****************************************************************/
namespace XiaoFeng.Log
{
    /// <summary>
    /// 日志基础类
    /// </summary>
    public abstract class BaseLog : Disposable, ILog
    {
        #region 构造器
        /// <summary>
        /// 无参构造器
        /// </summary>
        public BaseLog()
        {
            this.Config = LoggerConfig.Current;

            this.LogPath = this.Config.Path.IfEmpty("Log");
            this.StorageType = this.Config.StorageType;
            this.ConnectionConfig = new ConnectionConfig(this.Config.ConnectionStringKey);
            this.MessageType = this.Config.MessageType;
            this.LogLevel = this.Config.LogLevel;
            this.ConsoleFlags = this.Config.ConsoleFlags;
            //this.Signal = new SemaphoreSlim(this.Config.MaxThreads);
            /*初始化数据*/
            //var setting = Setting.Current;
            //if (setting.MaxWorkerThreads > 10)
            //ThreadPool.SetMaxThreads(setting.MaxWorkerThreads, setting.MaxWorkerThreads);
            //ThreadPool.SetMinThreads(1, 1);
        }
        #endregion

        #region 属性
        /// <summary>
        /// 日志路径
        /// </summary>
        public string LogPath { get; set; }
        /// <summary>
        /// 存储类型
        /// </summary>
        public StorageType StorageType { get; set; }
        /// <summary>
        /// 日志级别
        /// </summary>
        public LogType LogLevel { get; set; }
        /// <summary>
        /// 输出控制台标识
        /// </summary>
        public LogType ConsoleFlags { get; set; }
        /// <summary>
        /// 数据库连接配置
        /// </summary>
        internal ConnectionConfig ConnectionConfig { get; set; }
        /// <summary>
        /// 消息类型类型
        /// </summary>
        public EnumValueType MessageType { get; set; } = EnumValueType.Description;
        /// <summary>
        /// 日志配置
        /// </summary>
        public LoggerConfig Config { get; set; }
        #endregion

        #region 方法
        /// <summary>
        /// 信息
        /// </summary>
        /// <param name="message">消息</param>
        public virtual void Info(string message) => this.Write(new LogData
        {
            LogType = LogType.Info,
            Message = message
        });
        /// <summary>
        /// 错误
        /// </summary>
        /// <param name="ex">错误</param>
        /// <param name="message"></param>
        public virtual void Error(Exception ex, string message = "")
        {
            ex = ex ?? new Exception();
            this.Write(new LogData
            {
                LogType = LogType.Error,
                Message = ex?.Message + (message.IsNullOrEmpty() ? "" : "[自定义信息:' {0} ']".format(message)),
                DataSource = ex?.Source,
                ClassName = ex?.TargetSite == null ? "" : ex?.TargetSite?.DeclaringType?.Name,
                FunctionName = ex?.TargetSite == null ? "" : ex?.TargetSite?.Name,
                StackTrace = ex?.StackTrace ?? ""
            });
        }
        /// <summary>
        /// 调试
        /// </summary>
        /// <param name="message">消息</param>
        public virtual void Debug(string message) => this.Write(new LogData
        {
            LogType = LogType.Debug,
            Message = message
        });
        /// <summary>
        /// SQL
        /// </summary>
        /// <param name="message">消息</param>
        public virtual void SQL(string message) => this.Write(new LogData
        {
            LogType = LogType.SQL,
            Message = message
        });
        /// <summary>
        /// 警告
        /// </summary>
        /// <param name="message">消息</param>
        public virtual void Warn(string message) => this.Write(new LogData
        {
            LogType = LogType.Warn,
            Message = message
        });
        /// <summary>
        /// 任务
        /// </summary>
        /// <param name="message">消息</param>
        public virtual void Task(string message) => this.Write(new LogData
        {
            LogType = LogType.Task,
            Message = message
        });
        /// <summary>
        /// 跟踪
        /// </summary>
        /// <param name="message">消息</param>
        public virtual void Trace(string message) => this.Write(new LogData
        {
            LogType = LogType.Trace,
            Message = message
        });
        /// <summary>
        /// 记录
        /// </summary>
        /// <param name="message">消息</param>
        /// <param name="type">类型</param>
        public virtual void Record(string message, LogType type = LogType.Info) => this.Write(new LogData
        {
            LogType = type,
            Message = message,
            IsRecord = true
        });

        #region 记录日志
        /// <summary>
        /// 记录日志
        /// </summary>
        /// <param name="log">日志</param>
        public virtual void Write(LogData log)
        {
            /*if (log.RequestUrl.IsNotNullOrEmpty() && Web.HttpContext.Current != null && Web.HttpContext.Current.Request != null)
                log.RequestUrl = Web.HttpContext.Current.Request.GetUri().ToString();
            var trace = new StackTrace();
            ThreadPool.QueueUserWorkItem((o) =>
            {
                StackTrace stack = o as StackTrace;
                Thread.CurrentThread.IsBackground = true;
                if (log.Tracking == null) log.Tracking = stack;
                Run(log);
            }, trace);*/
            if (log.Tracking == null)
                log.Tracking = new StackTrace();
            Run(log);
        }
        /// <summary>
        /// 写数据
        /// </summary>
        /// <param name="log">消息内容</param>
        public abstract void Run(LogData log);
        #endregion

        #endregion
    }
}
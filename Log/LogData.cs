using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XiaoFeng.Config;

/****************************************************************
*  Copyright © (2021) www.fayelf.com All Rights Reserved.       *
*  Author : jacky                                               *
*  QQ : 7092734                                                 *
*  Email : jacky@fayelf.com                                     *
*  Site : www.fayelf.com                                        *
*  Create Time : 2021-05-08 17:38:23                            *
*  Version : v 1.0.0                                            *
*  CLR Version : 4.0.30319.42000                                *
*****************************************************************/
namespace XiaoFeng.Log
{
    /// <summary>
    /// 日志数据操作类
    /// </summary>
    public class LogData : EntityBase
    {
        #region 构造器
        /// <summary>
        /// 无参构造器
        /// </summary>
        public LogData()
        {

        }
        /// <summary>
        /// 设置自定义数据
        /// </summary>
        /// <param name="customMesssage">自定义数据</param>
        public LogData(string customMesssage)
        {
            this.CustomMessage = customMesssage;
            this.LogType = LogType.Info;
        }
        #endregion

        #region 属性
        /// <summary>
        /// 自定义信息
        /// </summary>
        private string CustomMessage { get; set; }
        /// <summary>
        /// 错误ID
        /// </summary>
        [Description("错误ID")]
        public string ErrorID { get; set; }
        /// <summary>
        /// 方法名
        /// </summary>
        [Description("方法名")]
        public string FunctionName { get; set; }
        /// <summary>
        /// 类名
        /// </summary>
        [Description("类名")]
        public string ClassName { get; set; }
        /// <summary>
        /// 信息
        /// </summary>
        [Description("信息")]
        public string Message { get; set; }
        /// <summary>
        /// 错误源
        /// </summary>
        [Description("错误源")]
        public string DataSource { get; set; }
        /// <summary>
        /// 日志类型
        /// </summary>
        [Description("日志类型")]
        public LogType LogType { get; set; }
        /// <summary>
        /// 是否去除前缀
        /// </summary>
        public Boolean IsReplace { get; set; }
        /// <summary>
        /// 错误堆栈
        /// </summary>
        [Description("错误堆栈")]
        public string StackTrace { get; set; }
        /// <summary>
        /// 错误堆栈
        /// </summary>
        [Description("堆栈跟踪")]
        public StackTrace Tracking { get; set; }
        /// <summary>
        /// 请求地址
        /// </summary>
        [Description("请求地址")]
        public string RequestUrl { get; set; }
        /// <summary>
        /// 日志时间
        /// </summary>
        [Description("日志时间")]
        public DateTime AddTime { get; set; }
        /// <summary>
        /// 是否记录
        /// </summary>
        [Description("是否记录")] 
        public Boolean IsRecord { get; set; } = false;
        /// <summary>
        /// 输出字段
        /// </summary>
        private string Fields { get; set; }
        /// <summary>
        /// 日志路径
        /// </summary>
        [Description("日志路径")]
        public string FilePath { get; set; }
        /// <summary>
        /// IO锁
        /// </summary>
        private static object IOLock = new object();
        #endregion

        #region 方法
        /// <summary>
        /// 转为字符串
        /// </summary>
        /// <param name="loggerConfig">日志配置</param>
        /// <returns></returns>
        public string ToString(LoggerConfig loggerConfig)
        {
            if (this.CustomMessage.IsNotNullOrEmpty())
            {
                return this.CustomMessage;
            }
            if (Fields.IsNullOrEmpty()) Fields = loggerConfig.Fields;
            var sb = new StringBuilder();
            if (this.IsRecord)
            {
                if (this.Message.IsNotNullOrEmpty())
                {
                    sb.AppendLine($"{this.LogType.GetValue(loggerConfig.MessageType)}".PadRight(loggerConfig.MessageType == EnumValueType.Description ? 2 : 5, ' ') + (Fields.Contains("AddTime") ? DateTime.Now.ToString(": yyyy-MM-dd HH:mm:ss.fff") : ""));
                    sb.AppendLine(this.Message);
                }
            }
            else
            {
                if (Fields.Contains("LogType"))
                {
                    sb.AppendLine("".PadLeft(70, '='));
                    sb.AppendLine($"{this.LogType.GetValue(loggerConfig.MessageType)}".PadRight(loggerConfig.MessageType == EnumValueType.Description ? 2 : 5, ' ') + (Fields.Contains("AddTime") ? DateTime.Now.ToString(": yyyy-MM-dd HH:mm:ss.fff") : ""));
                    sb.AppendLine("".PadLeft(70, '='));
                }
                if (this.DataSource.IsNotNullOrEmpty() && (Fields.Length == 0 || (Fields.Length > 0 && Fields.Contains("StackTrace")))) sb.AppendLine("数 据 源: " + this.DataSource);
                if (this.FunctionName.IsNotNullOrEmpty() && (Fields.Length == 0 || (Fields.Length > 0 && Fields.Contains("FunctionName")))) sb.AppendLine("方 法 名: " + this.ClassName + "." + this.FunctionName);
                if (this.Message.IsNotNullOrEmpty() && (Fields.Length == 0 || (Fields.Length > 0 && Fields.Contains("Message")))) sb.AppendLine("日志信息: " + this.Message);
                if (this.StackTrace.IsNotNullOrEmpty() && (Fields.Length == 0 || (Fields.Length > 0 && Fields.Contains("StackTrace")))) sb.AppendLine("日志堆栈: " + this.StackTrace);
                if ((LogType.Error | LogType.Trace).HasFlag(this.LogType) && this.Tracking.IsNotNullOrEmpty() && (Fields.Length == 0 || (Fields.Length > 0 && Fields.Contains("Tracking"))))
                {
                    sb.AppendLine("堆栈跟踪".PadLeft(33, '*').PadRight(66, '*'));
                    var trace = this.Tracking ?? new StackTrace();
                    trace.GetFrames().Each(frame =>
                    {
                        System.Reflection.MethodBase method = frame?.GetMethod();
                        if (method == null || method.DeclaringType?.FullName.IndexOf("LogHelper") > -1) return;
                        sb.AppendLine("{0}.{1}({2});".format(method.DeclaringType?.FullName, method.Name, method.GetParameters().Join(",")));
                        return;
                    });
                    sb.AppendLine("".PadLeft(70, '*'));
                }
            }
            return sb.ToString();
        }
        /// <summary>
        /// 控制台输出
        /// </summary>
        /// <param name="loggerConfig">日志配置</param>
        public void ConsoleOutput(LoggerConfig loggerConfig)
        {
            lock (IOLock)
            {
                if (this.CustomMessage.IsNotNullOrEmpty())
                {
                    Console.WriteLine(this.CustomMessage);
                    return;
                }
                if (Fields.IsNullOrEmpty()) Fields = loggerConfig.Fields;
                var ForegroundColor = (int)Console.ForegroundColor;
                var BackgroundColor = (int)Console.BackgroundColor;
                var foreColor = Console.ForegroundColor;
                var backColor = Console.BackgroundColor;
                switch (this.LogType)
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
                if (this.IsRecord)
                {
                    if (this.Message.IsNotNullOrEmpty())
                    {
                        Console.WriteLine($"{this.LogType.GetValue(loggerConfig.MessageType)}".PadRight(loggerConfig.MessageType == EnumValueType.Description ? 2 : 5, ' ') + (Fields.Contains("AddTime") ? DateTime.Now.ToString(": yyyy-MM-dd HH:mm:ss.fff") : ""));
                        Console.ResetColor();
                        Console.WriteLine(this.Message);
                    }
                }
                else
                {
                    if (Fields.Contains("LogType"))
                    {
                        Console.WriteLine("".PadLeft(70, '='));
                        Console.WriteLine($"{this.LogType.GetValue(loggerConfig.MessageType)}".PadRight(loggerConfig.MessageType == EnumValueType.Description ? 2 : 5, ' ') + (Fields.Contains("AddTime") ? DateTime.Now.ToString(": yyyy-MM-dd HH:mm:ss.fff") : ""));
                        Console.WriteLine("".PadLeft(70, '='));
                    }
                    Console.ResetColor();
                    if (this.DataSource.IsNotNullOrEmpty() && (Fields.Length == 0 || (Fields.Length > 0 && Fields.Contains("DataSource"))))
                    {
                        Console.WriteLine("数 据 源: " + this.DataSource);
                    }
                    if (this.FunctionName.IsNotNullOrEmpty() && (Fields.Length == 0 || (Fields.Length > 0 && Fields.Contains("FunctionName"))))
                    {
                        Console.WriteLine("方 法 名: " + this.ClassName + "." + this.FunctionName);
                    }
                    if (this.Message.IsNotNullOrEmpty() && (Fields.Length == 0 || (Fields.Length > 0 && Fields.Contains("Message"))))
                    {
                        Console.WriteLine((this.LogType == LogType.Warn ? "" : "日志信息: ") + this.Message);
                    }
                    if (this.StackTrace.IsNotNullOrEmpty() && (Fields.Length == 0 || (Fields.Length > 0 && Fields.Contains("StackTrace"))))
                    {
                        Console.WriteLine("日志堆栈: " + this.StackTrace);
                    }
                    if ((LogType.Error | LogType.Trace).HasFlag(this.LogType) && this.Tracking.IsNotNullOrEmpty() && (Fields.Length == 0 || (Fields.Length > 0 && Fields.Contains("Tracking"))))
                    {
                        Console.WriteLine("堆栈跟踪".PadLeft(33, '*').PadRight(66, '*'));
                        var trace = this.Tracking ?? new StackTrace();
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
        /// <summary>
        /// 输出SQL语句
        /// </summary>
        /// <param name="loggerConfig">日志配置</param>
        /// <returns></returns>
        public string ToSqlString(LoggerConfig loggerConfig)
        {
            if (loggerConfig.InsertSql.IsNotNullOrEmpty())
            {
                if (this.CustomMessage.IsNotNullOrEmpty())
                {
                    return loggerConfig.InsertSql.format(this.CustomMessage);
                }
                var Tracking = "堆栈跟踪".PadLeft(33, '*').PadRight(66, '*');
                var trace = this.Tracking ?? new StackTrace();
                trace.GetFrames().Each(frame =>
                {
                    System.Reflection.MethodBase method = frame?.GetMethod();
                    if (method == null || method.DeclaringType?.FullName.IndexOf("LogHelper") > -1) return;
                    var _ = "{0}.{1}({2});".format(method.DeclaringType?.FullName, method.Name, method.GetParameters().Join(","));
                    Tracking += _;
                    return;
                });
                Tracking += "".PadLeft(70, '*');
                var sql = loggerConfig.InsertSql.ReplacePattern(@"\{(?<a>[a-z]+)\}", m =>
                {
                    var name = m.Groups["a"].Value;
                    return name.EqualsIgnoreCase("Tracking") ? Tracking : this[name].ToString();
                });
                return sql;
            }
            return string.Empty;
        }
        #endregion
    }
}
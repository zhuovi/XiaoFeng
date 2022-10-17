using System;
using System.Collections.Generic;
using System.Text;
using XiaoFeng.Log;

/****************************************************************
*  Copyright © (2022) www.fayelf.com All Rights Reserved.       *
*  Author : jacky                                               *
*  QQ : 7092734                                                 *
*  Email : jacky@fayelf.com                                     *
*  Site : www.fayelf.com                                        *
*  Create Time : 2022-08-04 11:57:17                            *
*  Version : v 1.0.0                                            *
*  CLR Version : 4.0.30319.42000                                *
*****************************************************************/
namespace XiaoFeng.Log
{
    /// <summary>
    /// 控制台输出扩展
    /// </summary>
    public static class ConsoleX
    {

        #region 属性

        #endregion

        #region 方法
        /// <summary>
        /// 输出控制台
        /// </summary>
        /// <param name="logType">日志类型</param>
        /// <param name="msg">消息</param>
        public static void Write(LogType logType, string msg)
        {
            var foreColor = Console.ForegroundColor;
            var backColor = Console.BackgroundColor;
            switch (logType)
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
            Console.WriteLine(msg);
            Console.ResetColor();
        }
        /// <summary>
        /// 调试
        /// </summary>
        /// <param name="msg">消息</param>
        public static void Debug(string msg)
        {
            msg = $"{"".PadLeft(70, '=')}{Environment.NewLine}{msg}{Environment.NewLine}{"".PadLeft(70, '=')}{Environment.NewLine}";
            Write(LogType.Debug, msg);
        }
        /// <summary>
        /// 信息
        /// </summary>
        /// <param name="msg">消息</param>
        public static void Info(string msg) => Write(LogType.Info, msg);
        /// <summary>
        /// 错误
        /// </summary>
        /// <param name="msg">消息</param>
        public static void Error(string msg) => Write(LogType.Error, msg);
        /// <summary>
        /// 警告
        /// </summary>
        /// <param name="msg">消息</param>
        public static void Warn(string msg) => Write(LogType.Warn, msg);
        /// <summary>
        /// SQL
        /// </summary>
        /// <param name="msg">消息</param>
        public static void SQL(string msg) => Write(LogType.SQL, msg);
        /// <summary>
        /// 任务
        /// </summary>
        /// <param name="msg">消息</param>
        public static void Task(string msg) => Write(LogType.Task, msg);
        /// <summary>
        /// 堆栈
        /// </summary>
        /// <param name="msg">消息</param>
        public static void Trace(string msg) => Write(LogType.Trace, msg);
        #endregion
    }
}
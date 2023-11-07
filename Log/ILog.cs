using System;

/****************************************************************
 *  Copyright © (2021) www.fayelf.com All Rights Reserved.      *
 *  Author : jacky                                              *
 *  QQ : 7092734                                                *
 *  Email : jacky@fayelf.com                                    *
 *  Site : www.fayelf.com                                       *
 *  Create Time : 2021-05-07 11:03:15                           *
 *  Version : v 1.0.0                                           *
 *  CLR Version : 4.0.30319.42000                               *
 ****************************************************************/
namespace XiaoFeng.Log
{
    /// <summary>
    /// 日志接口
    /// </summary>
    public interface ILog
    {
        #region 属性
        /// <summary>
        /// 路径
        /// </summary>
        string LogPath { get; set; }
        /// <summary>
        /// 存储类型
        /// </summary>
        StorageType StorageType { get; set; }
        /// <summary>
        /// 日志级别
        /// </summary>
        LogType LogLevel { get; set; }
        /// <summary>
        /// 输出控制台标识
        /// </summary>
        LogType ConsoleFlags { get; set; }
        #endregion

        #region 方法
        /// <summary>
        /// 写日志
        /// </summary>
        /// <param name="log">日志</param>
        void Write(LogData log);
        /// <summary>
        /// SQL
        /// </summary>
        /// <param name="message">消息</param>
        void SQL(string message);
        /// <summary>
        /// 错误
        /// </summary>
        /// <param name="ex">异常</param>
        /// <param name="message">消息</param>
        void Error(Exception ex, string message);
        /// <summary>
        /// 调试
        /// </summary>
        /// <param name="message">消息</param>
        void Debug(string message);
        /// <summary>
        /// 警告
        /// </summary>
        /// <param name="message">消息</param>
        void Warn(string message);
        /// <summary>
        /// 信息
        /// </summary>
        /// <param name="message">消息</param>
        void Info(string message);
        /// <summary>
        /// 记录
        /// </summary>
        /// <param name="message">消息</param>
        /// <param name="type">类型</param>
        void Record(string message, LogType type = LogType.Info);
        #endregion
    }
}
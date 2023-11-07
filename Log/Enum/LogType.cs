using System;
using System.ComponentModel;

/****************************************************
 *  Copyright © www.fayelf.com All Rights Reserved  *
 *  Author : jacky                                  *
 *  QQ : 7092734                                    *
 *  Email : jacky@fayelf.com                        *
 *  Site : www.fayelf.com                           *
 *  Create Time : 2021/1/12 10:00:18          *
 *  Version : v 1.0.0                               *
 ****************************************************/
namespace XiaoFeng.Log
{
    #region 枚举
    /// <summary>
    /// 错误类型
    /// </summary>
    [Flags]
    public enum LogType
    {
        /// <summary>
        /// 跟踪
        /// </summary>
        [Description("跟踪")]
        Trace = 1 << 0,
        /// <summary>
        /// 调试
        /// </summary>
        [Description("调试")]
        Debug = 1 << 1,
        /// <summary>
        /// SQL
        /// </summary>
        [Description("SQL")]
        SQL = 1 << 2,
        /// <summary>
        /// 任务
        /// </summary>
        [Description("任务")]
        Task = 1 << 3,
        /// <summary>
        /// 信息
        /// </summary>
        [Description("信息")]
        Info = 1 << 4,
        /// <summary>
        /// 警告
        /// </summary>
        [Description("警告")]
        Warn = 1 << 5,
        /// <summary>
        /// 出错
        /// </summary>
        [Description("出错")]
        Error = 1 << 6
    }
    #endregion
}
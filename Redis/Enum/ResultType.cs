using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
/****************************************************************
*  Copyright © (2022) www.fayelf.com All Rights Reserved.       *
*  Author : jacky                                               *
*  QQ : 7092734                                                 *
*  Email : jacky@fayelf.com                                     *
*  Site : www.fayelf.com                                        *
*  Create Time : 2022-08-12 16:33:36                            *
*  Version : v 1.0.0                                            *
*  CLR Version : 4.0.30319.42000                                *
*****************************************************************/
namespace XiaoFeng.Redis
{
    /// <summary>
    /// 结果类型
    /// </summary>
    public enum ResultType
    {
        /// <summary>
        /// 未知
        /// </summary>
        [Description("未知")] 
        Unknow = 0,
        /// <summary>
        /// 错误消息
        /// </summary>
        [Description("错误消息")]
        Error = '-',
        /// <summary>
        /// 状态消息
        /// </summary>
        [Description("状态消息")] 
        Status = '+',
        /// <summary>
        /// 单条消息
        /// </summary>
        [Description("单条消息")] 
        Bulk = '$',
        /// <summary>
        /// 多条消息
        /// </summary>
        [Description("多条消息")] 
        MultiBulk = '*',
        /// <summary>
        /// 整型消息
        /// </summary>
        [Description("整型消息")] 
        Int = ':'
    }
}
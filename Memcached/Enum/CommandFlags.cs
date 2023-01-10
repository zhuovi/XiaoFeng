using System;
using System.Collections.Generic;
using System.Text;

/****************************************************************
*  Copyright © (2023) www.fayelf.com All Rights Reserved.       *
*  Author : jacky                                               *
*  QQ : 7092734                                                 *
*  Email : jacky@fayelf.com                                     *
*  Site : www.fayelf.com                                        *
*  Create Time : 2023-01-09 16:55:16                            *
*  Version : v 1.0.0                                            *
*  CLR Version : 4.0.30319.42000                                *
*****************************************************************/
namespace XiaoFeng.Memcached
{
    /// <summary>
    /// 命令格式
    /// </summary>
    public enum CommandFlags
    {
        /// <summary>
        /// 未知
        /// </summary>
        [Range(-1, -1)] 
        Unknow = 0,
        /// <summary>
        /// 存储
        /// </summary>
        [Range(0,19)]
        Store = 1,
        /// <summary>
        /// 检索
        /// </summary>
        [Range(20, 39)] 
        Get = 2,
        /// <summary>
        /// 其它
        /// </summary>
        [Range(40, 59)] 
        Other = 3,
        /// <summary>
        /// 统计
        /// </summary>
        [Range(60, 79)]
        Stats = 4
    }
}
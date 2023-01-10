using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
/****************************************************************
*  Copyright © (2023) www.fayelf.com All Rights Reserved.       *
*  Author : jacky                                               *
*  QQ : 7092734                                                 *
*  Email : jacky@fayelf.com                                     *
*  Site : www.fayelf.com                                        *
*  Create Time : 2023-01-07 09:35:59                            *
*  Version : v 1.0.0                                            *
*  CLR Version : 4.0.30319.42000                                *
*****************************************************************/
namespace XiaoFeng.Memcached
{
    /// <summary>
    /// 统计命名
    /// </summary>
    public enum StatsCommand
    {
        /// <summary>
        /// 返回统计信息
        /// </summary>
        [Description("返回统计信息")]
        Stats = 40,
        /// <summary>
        /// 显示各个 slab 中 item 的数目和存储时长(最后一次访问距离现在的秒数)
        /// </summary>
        [Description("显示各个 slab 中 item 的数目和存储时长")]
        [EnumName("Stats Items")]
        Items = 41,
        /// <summary>
        /// 显示各个slab的信息，包括chunk的大小、数目、使用情况等
        /// </summary>
        [Description("显示各个slab的信息，包括chunk的大小、数目、使用情况等")]
        [EnumName("Stats Slabs")] 
        Slabs = 42,
        /// <summary>
        /// 显示所有item的大小和个数
        /// </summary>
        [Description("显示所有item的大小和个数")]
        [EnumName("Stats Sizes")] 
        Sizes = 43,
        /// <summary>
        /// 用于清理缓存中的所有 key=>value(键=>值) 对
        /// </summary>
        [Description("用于清理缓存中的所有 key=>value(键=>值) 对")]
        [EnumName("flush_all")] 
        FlushAll = 44
    }
}
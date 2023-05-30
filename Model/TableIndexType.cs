using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

/****************************************************************
*  Copyright © (2023) www.fayelf.com All Rights Reserved.       *
*  Author : jacky                                               *
*  QQ : 7092734                                                 *
*  Email : jacky@fayelf.com                                     *
*  Site : www.fayelf.com                                        *
*  Create Time : 2023-05-19 15:29:19                            *
*  Version : v 1.0.0                                            *
*  CLR Version : 4.0.30319.42000                                *
*****************************************************************/
namespace XiaoFeng.Model
{
    /// <summary>
    /// 索引类型
    /// </summary>
    public enum TableIndexType
    {
        /// <summary>
        /// 聚簇索引
        /// </summary>
        [Description("聚簇索引")]
        Clustered = 0,
        /// <summary>
        /// 非聚簇索引
        /// </summary>
        [Description("非聚簇索引")] 
        NonClustered = 1,
        /// <summary>
        /// 唯一索引 
        /// </summary>
        [Description("唯一索引")] 
        Unique = 2
    }
}
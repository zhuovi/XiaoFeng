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
*  Create Time : 2023-01-07 09:15:37                            *
*  Version : v 1.0.0                                            *
*  CLR Version : 4.0.30319.42000                                *
*****************************************************************/
namespace XiaoFeng.Memcached
{
    /// <summary>
    /// 存储命令
    /// </summary>
    public enum StoreCommand
    {
        /// <summary>
        /// 给key设置一个值
        /// </summary>
        [Description("给key设置一个值")]
        Set = 0,
        /// <summary>
        /// 如果key不存在的话，就添加
        /// </summary>
        [Description("如果key不存在的话，就添加")]
        Add = 1,
        /// <summary>
        /// 用来替换已知key的value
        /// </summary>
        [Description("用来替换已知key的value")]
        Replace = 2,
        /// <summary>
        /// 表示将提供的值附加到现有key的value之后，是一个附加操作
        /// </summary>
        [Description("表示将提供的值附加到现有key的value之后，是一个附加操作")] 
        Append = 3,
        /// <summary>
        /// 将此数据添加到现有数据之前的现有键中
        /// </summary>
        [Description("将此数据添加到现有数据之前的现有键中")] 
        Prepend = 4,
        /// <summary>
        /// 一个原子操作，只有当casunique匹配的时候，才会设置对应的值
        /// </summary>
        [Description("一个原子操作，只有当casunique匹配的时候，才会设置对应的值")] 
        Cas = 5
    }
}
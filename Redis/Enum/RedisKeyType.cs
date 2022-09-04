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
*  Create Time : 2022-08-16 10:51:40                            *
*  Version : v 1.0.0                                            *
*  CLR Version : 4.0.30319.42000                                *
*****************************************************************/
namespace XiaoFeng.Redis
{
    /// <summary>
    /// Redis 结构类型
    /// </summary>
    public enum RedisKeyType
    {
        /// <summary>
        /// 不存在
        /// </summary>
        [Description("不存在")]
        None = -2,
        /// <summary>
        /// 错误
        /// </summary>
        [Description("错误")]
        Error = -1,
        /// <summary>
        /// 成功
        /// </summary>
        [Description("成功")]
        OK = 0,
        /// <summary>
        /// 字符串
        /// </summary>
        [Description("字符串")]
        String  =1,
        /// <summary>
        /// 哈希
        /// </summary>
        [Description("哈希")]
        Hash = 2,
        /// <summary>
        /// 列表
        /// </summary>
        [Description("列表")]
        List = 3,
        /// <summary>
        /// 集合
        /// </summary>
        [Description("集合")]
        Set = 4,
        /// <summary>
        /// 有序集合
        /// </summary>
        [Description("有序集合")]
        ZSet = 5
    }
}
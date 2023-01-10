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
*  Create Time : 2023-01-07 09:23:14                            *
*  Version : v 1.0.0                                            *
*  CLR Version : 4.0.30319.42000                                *
*****************************************************************/
namespace XiaoFeng.Memcached
{
    /// <summary>
    /// 读取命令
    /// </summary>
    public enum GetCommand
    {
        /// <summary>
        /// 获取key的value值，若key不存在，返回空。支持多个key
        /// </summary>
        [Description("获取key的value值，若key不存在，返回空。支持多个key")]
        Get = 20,
        /// <summary>
        /// 用于获取key的带有CAS令牌值的value值，若key不存在，返回空。支持多个key
        /// </summary>
        [Description("用于获取key的带有CAS令牌值的value值，若key不存在，返回空。支持多个key")]
        Gets = 21,
        /// <summary>
        /// 获取key的value值，若key不存在，返回空。支持多个key 更新缓存时间
        /// </summary>
        [Description("获取key的value值，若key不存在，返回空。支持多个key 更新缓存时间")]
        Gat = 22,
        /// <summary>
        /// 用于获取key的带有CAS令牌值的value值，若key不存在，返回空。支持多个key 更新缓存时间
        /// </summary>
        [Description("用于获取key的带有CAS令牌值的value值，若key不存在，返回空。支持多个key 更新缓存时间")]
        Gats = 23,
        /// <summary>
        /// 删除Key
        /// </summary>
        [Description("删除Key")]
        [EnumName("delete")]
        Delete = 24,
        /// <summary>
        /// 递增
        /// </summary>
        [Description("递增")]
        [EnumName("Incr")]
        Increment = 25,
        /// <summary>
        /// 递减
        /// </summary>
        [Description("递减")]
        [EnumName("Decr")]
        Decrement = 26,
        /// <summary>
        /// 修改key过期时间
        /// </summary>
        [Description("修改key过期时间")]
        [EnumName("touch")]
        Touch = 27
    }
}
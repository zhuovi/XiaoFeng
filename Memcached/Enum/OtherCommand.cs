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
*  Create Time : 2023-01-07 09:28:45                            *
*  Version : v 1.0.0                                            *
*  CLR Version : 4.0.30319.42000                                *
*****************************************************************/
namespace XiaoFeng.Memcached
{
    /// <summary>
    /// 其它命令
    /// </summary>
    public enum OtherCommand
    {
        /// <summary>
        /// 删除Key
        /// </summary>
        [Description("删除Key")]
        [EnumName("delete")]
        Delete = 40,
        /// <summary>
        /// 递增
        /// </summary>
        [Description("递增")]
        [EnumName("Incr")]
        Increment = 41,
        /// <summary>
        /// 递减
        /// </summary>
        [Description("递减")]
        [EnumName("Decr")]
        Decrement = 42,
        /// <summary>
        /// 修改key过期时间
        /// </summary>
        [Description("修改key过期时间")]
        [EnumName("touch")]
        Touch = 43
    }
}
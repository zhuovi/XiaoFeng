using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

/****************************************************************
*  Copyright © (2023) www.eelf.cn All Rights Reserved.          *
*  Author : jacky                                               *
*  QQ : 7092734                                                 *
*  Email : jacky@eelf.cn                                        *
*  Site : www.eelf.cn                                           *
*  Create Time : 2023-09-13 17:07:43                            *
*  Version : v 1.0.0                                            *
*  CLR Version : 4.0.30319.42000                                *
*****************************************************************/
namespace XiaoFeng.Memcached.Protocol.Binary
{
    /// <summary>
    /// 魔法类型
    /// </summary>
    public enum MagicType
    {
        /// <summary>
        /// 请求
        /// </summary>
        [Description("请求")]
        Request = 0x80,
        /// <summary>
        /// 响应
        /// </summary>
        [Description("响应")]
        Response = 0x81
    }
}
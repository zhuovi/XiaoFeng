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
*  Create Time : 2023-01-06 11:31:52                            *
*  Version : v 1.0.0                                            *
*  CLR Version : 4.0.30319.42000                                *
*****************************************************************/
namespace XiaoFeng.Memcached
{
    /// <summary>
    /// Memcached协议
    /// </summary>
    public enum MemcachedProtocol
    {
        /// <summary>
        /// 文本协议
        /// </summary>
        [Description("文本协议")]
        Text = 0,
        /// <summary>
        /// 二进制协议
        /// </summary>
        [Description("二进制协议")]
        Binary = 1
    }
}
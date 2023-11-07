﻿using System;

/****************************************************************
*  Copyright © (2023) www.eelf.cn All Rights Reserved.          *
*  Author : jacky                                               *
*  QQ : 7092734                                                 *
*  Email : jacky@eelf.cn                                        *
*  Site : www.eelf.cn                                           *
*  Create Time : 2023-09-15 11:51:38                            *
*  Version : v 1.0.0                                            *
*  CLR Version : 4.0.30319.42000                                *
*****************************************************************/
namespace XiaoFeng.Memcached.Attributes
{
    /// <summary>
    /// 二进制协议
    /// </summary>
    [AttributeUsage(AttributeTargets.Field)]
    public class ProtocolBinaryAttribute : Attribute
    {
        #region 构造器
        /// <summary>
        /// 无参构造器
        /// </summary>
        public ProtocolBinaryAttribute()
        {

        }
        #endregion

        #region 属性

        #endregion

        #region 方法

        #endregion
    }
}
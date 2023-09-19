using System;
using System.Collections.Generic;
using System.Text;
using XiaoFeng.Memcached.IO;

/****************************************************************
*  Copyright © (2023) www.eelf.cn All Rights Reserved.          *
*  Author : jacky                                               *
*  QQ : 7092734                                                 *
*  Email : jacky@eelf.cn                                        *
*  Site : www.eelf.cn                                           *
*  Create Time : 2023-09-15 11:06:15                            *
*  Version : v 1.0.0                                            *
*  CLR Version : 4.0.30319.42000                                *
*****************************************************************/
namespace XiaoFeng.Memcached.Protocol.Text
{
    /// <summary>
    /// 基础包
    /// </summary>
    public class BasePaket
    {
        #region 构造器
        /// <summary>
        /// 无参构造器
        /// </summary>
        public BasePaket()
        {

        }
        #endregion

        #region 属性
        /// <summary>
        /// Socket
        /// </summary>
        public IMemcachedSocket MemcachedSocket { get; set; }
        /// <summary>
        /// 压缩包长度
        /// </summary>
        public uint CompressLength { get; set; }
        #endregion

        #region 方法

        #endregion
    }
}
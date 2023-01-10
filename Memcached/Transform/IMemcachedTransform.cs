using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

/****************************************************************
*  Copyright © (2023) www.fayelf.com All Rights Reserved.       *
*  Author : jacky                                               *
*  QQ : 7092734                                                 *
*  Email : jacky@fayelf.com                                     *
*  Site : www.fayelf.com                                        *
*  Create Time : 2023-01-09 14:34:00                            *
*  Version : v 1.0.0                                            *
*  CLR Version : 4.0.30319.42000                                *
*****************************************************************/
namespace XiaoFeng.Memcached.Transform
{
    /// <summary>
    /// 加密接口
    /// </summary>
    public interface IMemcachedTransform
    {
        /// <summary>
        /// 计算指定 System.IO.Stream 对象的哈希值。
        /// </summary>
        /// <param name="inputStream">要计算其哈希代码的输入。</param>
        /// <returns>计算所得的哈希代码。</returns>
        byte[] ComputeHash(Stream inputStream);
        /// <summary>
        /// 计算指定字节数组的指定区域的哈希值。
        /// </summary>
        /// <param name="buffer">要计算其哈希代码的输入。</param>
        /// <param name="offset">字节数组中的偏移量，从该位置开始使用数据。</param>
        /// <param name="count">数组中用作数据的字节数。</param>
        /// <returns>计算所得的哈希代码。</returns>
        byte[] ComputeHash(byte[] buffer, int offset, int count);
        /// <summary>
        /// 计算指定字节数组的哈希值。
        /// </summary>
        /// <param name="buffer">要计算其哈希代码的输入。</param>
        /// <returns>计算所得的哈希代码。</returns>
        byte[] ComputeHash(byte[] buffer);
    }
}
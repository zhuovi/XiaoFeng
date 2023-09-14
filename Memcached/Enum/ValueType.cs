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
*  Create Time : 2023-01-07 11:13:16                            *
*  Version : v 1.0.0                                            *
*  CLR Version : 4.0.30319.42000                                *
*****************************************************************/
namespace XiaoFeng.Memcached
{
    /// <summary>
    /// 值类型
    /// </summary>
    public enum ValueType
    {
        /// <summary>
        /// 字符串
        /// </summary>
        String = 0,
        /// <summary>
        /// 字节数组
        /// </summary>
        ByteArray = 1,
        /// <summary>
        /// 对象
        /// </summary>
        Object = 2,
        /// <summary>
        /// 时间
        /// </summary>
        Datetime = 3,
        /// <summary>
        /// 布尔
        /// </summary>
        Boolean = 4,
        /// <summary>
        /// 字节
        /// </summary>
        Byte = 5,
        /// <summary>
        /// 短数字
        /// </summary>
        Short = 6,
        /// <summary>
        /// 无符号短数字
        /// </summary>
        UShort = 7,
        /// <summary>
        /// 整型
        /// </summary>
        Int = 8,
        /// <summary>
        /// 无符号整型
        /// </summary>
        UInt = 9,
        /// <summary>
        /// 长整型
        /// </summary>
        Long = 10,
        /// <summary>
        /// 无符号长整型
        /// </summary>
        ULong = 11,
        /// <summary>
        /// 浮点
        /// </summary>
        Float = 12,
        /// <summary>
        /// 双精度
        /// </summary>
        Double = 13,
        /// <summary>
        /// 压缩字节数组
        /// </summary>
        CompressedByteArray = 100,
        /// <summary>
        /// 压缩对象
        /// </summary>
        CompressedObject = 101,
        /// <summary>
        /// 压缩字符串
        /// </summary>
        CompressedString = 102,
    }
}
using System;
using System.Collections.Generic;
using System.Text;
using XiaoFeng.Memcached.Internal;
using XiaoFeng.Memcached.IO;
using XiaoFeng.Net;

/****************************************************************
*  Copyright © (2023) www.eelf.cn All Rights Reserved.          *
*  Author : jacky                                               *
*  QQ : 7092734                                                 *
*  Email : jacky@eelf.cn                                        *
*  Site : www.eelf.cn                                           *
*  Create Time : 2023-09-14 13:31:32                            *
*  Version : v 1.0.0                                            *
*  CLR Version : 4.0.30319.42000                                *
*****************************************************************/
namespace XiaoFeng.Memcached.Protocol.Binary
{
    /*
     * https://github.com/memcached/memcached/blob/master/doc/protocol-binary.txt
    数据包通过格式
     Byte/     0       |       1       |       2       |       3       |
        /              |               |               |               |
       |0 1 2 3 4 5 6 7|0 1 2 3 4 5 6 7|0 1 2 3 4 5 6 7|0 1 2 3 4 5 6 7|
       +---------------+---------------+---------------+---------------+
      0/ HEADER                                                        /
       /                                                               /
       /                                                               /
       /                                                               /
       +---------------+---------------+---------------+---------------+
     24/ COMMAND-SPECIFIC EXTRAS (as needed)                           /
      +/  (note length in the extras length header field)              /
       +---------------+---------------+---------------+---------------+
      m/ Key (as needed)                                               /
      +/  (note length in key length header field)                     /
       +---------------+---------------+---------------+---------------+
      n/ Value (as needed)                                             /
      +/  (note length is total body length header field, minus        /
      +/   sum of the extras and key length body fields)               /
       +---------------+---------------+---------------+---------------+
       Total 24 bytes
     */
    /// <summary>
    /// 基础包
    /// </summary>
    public abstract class BasePacket
    {
        #region 构造器
        /// <summary>
        /// 无参构造器
        /// </summary>
        public BasePacket()
        {

        }
        #endregion

        #region 属性
        /// <summary>
        /// Socket
        /// </summary>
        public ISocketClient MemcachedSocket { get; set; }
        /// <summary>
        /// 配置
        /// </summary>
        public Internal.MemcachedConfig Config { get; set; }
        /// <summary>
        /// 魔法数字，用来区分包头是请求包头还是响应包头
        /// </summary>
        public MagicType Magic { get; set; }
        /// <summary>
        /// 操作码命令码，也就是对应的命令
        /// </summary>
        public CommandOpcode Opcode { get; set; }
        /// <summary>
        /// 保留字段 目前只有一个固定的值：0x00
        /// </summary>
        public int DataType { get; set; } = 0x00;
        /// <summary>
        /// 请求生成的一个数据，会被原封不动在对应的响应中返回
        /// </summary>
        public byte[] Opaque { get; set; } = new byte[4];
        /// <summary>
        /// 数据的一个唯一标记
        /// </summary>
        public byte[] CAS { get; set; } = new byte[8];
        /// <summary>
        /// 特殊额外命令
        /// </summary>
        public byte[] Extras { get; set; }
        /// <summary>
        /// Key
        /// </summary>
        public byte[] Key { get; set; }
        /// <summary>
        /// 值
        /// </summary>
        public byte[] Value { get; set; }
        #endregion

        #region 方法
        /// <summary>
        /// 字节转数字
        /// </summary>
        /// <param name="values">字节组</param>
        /// <returns></returns>
        public long ToValue(byte[] values)
        {
            if (values == null || values.Length == 0) return 0;
            var hexs = "";
            for (var i = 0; i < values.Length; i++)
                hexs += values[i].ToString("X");
            return Convert.ToInt64(hexs, 16);
        }
        /// <summary>
        /// 数值转字节
        /// </summary>
        /// <param name="val">数值</param>
        /// <param name="length">字节数组长度</param>
        /// <returns></returns>
        public byte[] ToBytes(long val, int length)
        {
            return val.ToString("X").PadLeft(2 * length, '0').HexStringToBytes();
        }
        #endregion
    }
}
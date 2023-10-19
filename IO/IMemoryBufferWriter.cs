using System;
using System.Collections.Generic;
using System.Text;

/****************************************************************
*  Copyright © (2023) www.eelf.cn All Rights Reserved.          *
*  Author : jacky                                               *
*  QQ : 7092734                                                 *
*  Email : jacky@eelf.cn                                        *
*  Site : www.eelf.cn                                           *
*  Create Time : 2023-10-19 17:19:00                            *
*  Version : v 1.0.0                                            *
*  CLR Version : 4.0.30319.42000                                *
*****************************************************************/
namespace XiaoFeng.IO
{
    /// <summary>
    /// 内存流写入器接口
    /// </summary>
    public interface IMemoryBufferWriter : IMemoryBufferBase
    {
        /// <summary>
        /// 最大可变长度
        /// </summary>
        ulong VariableByteIntegerMaxValue { get; set; }
        /// <summary>
        /// 写字节
        /// </summary>
        /// <param name="byte">字节</param>
        void WriteByte(byte @byte);
        /// <summary>
        /// 写入数据
        /// </summary>
        /// <param name="bytes">数据</param>
        void WriteBytes(byte[] bytes);
        /// <summary>
        /// 写入数据
        /// </summary>
        /// <param name="bytes">数据</param>
        /// <param name="offset">开始位置</param>
        /// <param name="length">长度</param>
        void WriteBytes(byte[] bytes, int offset, int length);
        /// <summary>
        /// 获取可变字节长度占用字节数
        /// </summary>
        /// <param name="length">可变字节长度</param>
        /// <returns></returns>
        /// <exception cref="Exception">超过指定长度则抛出异常</exception>
        public int GetVariableByteIntegerSize(ulong length);
        /// <summary>
        /// 获取可变字节长度字节数据
        /// </summary>
        /// <param name="length">值</param>
        /// <exception cref="Exception">异常</exception>
        byte[] EncodeVariableByteInteger(ulong length);
        /// <summary>
        /// 把写入器写入当前写入器
        /// </summary>
        /// <param name="writer">写入器</param>
        void WriteBuffer(IMemoryBufferWriter writer);
        /// <summary>
        /// 把读取器剩下数据写入当前写入器
        /// </summary>
        /// <param name="reader">读取器</param>
        void WriteBuffer(IMemoryBufferReader reader);
    }
}
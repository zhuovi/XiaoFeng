using System;

/****************************************************************
*  Copyright © (2023) www.eelf.cn All Rights Reserved.          *
*  Author : jacky                                               *
*  QQ : 7092734                                                 *
*  Email : jacky@eelf.cn                                        *
*  Site : www.eelf.cn                                           *
*  Create Time : 2023-10-19 17:19:19                            *
*  Version : v 1.0.0                                            *
*  CLR Version : 4.0.30319.42000                                *
*****************************************************************/
namespace XiaoFeng.IO
{
    /// <summary>
    /// 内存流读取器接口
    /// </summary>
    public interface IMemoryBufferReader
    {
        /// <summary>
        /// 剩余未读长度
        /// </summary>
        long RemainingLength { get; }
        /// <summary>
        /// 往读取器中写入数据
        /// </summary>
        /// <param name="buffer">数据</param>
        void WriteBuffer(byte[] buffer);
        /// <summary>
        /// 往读取器中写入数据
        /// </summary>
        /// <param name="buffer">数据</param>
        /// <param name="offset">开始位置</param>
        /// <param name="length">长度</param>
        void WriteBuffer(byte[] buffer, int offset, int length);
        /// <summary>
        /// 读取可变小字节整型数字
        /// </summary>
        /// <returns></returns>
        uint DecodeVariableByteInteger();
        /// <summary>
        /// 读取可变小字节整型数字
        /// </summary>
        /// <returns></returns>
        /// <exception cref="Exception">异常</exception>
        uint DecodeVariableByteInteger(out int size);
        /// <summary>
        /// 从当前流中读取一个字节
        /// </summary>
        /// <returns></returns>
        int ReadByte();
        /// <summary>
        /// 读取指定长度字节
        /// </summary>
        /// <param name="length">长度 长度为0时读取剩下的所有字节</param>
        /// <returns></returns>
        byte[] ReadBytes(long length);
        /// <summary>
        /// 读取剩余所有数据
        /// </summary>
        /// <returns></returns>
        byte[] ReadBytes();
        /// <summary>
        /// 读取一行数据
        /// </summary>
        /// <returns></returns>
        byte[] ReadLine();
        /// <summary>
        /// 读取2个字节整型数字
        /// </summary>
        /// <returns></returns>
        ushort ReadTwoByteInteger();
        /// <summary>
        /// 读取4个字节整型数字
        /// </summary>
        /// <returns></returns>
        uint ReadFourByteInteger();
        /// <summary>
        /// 把写入器数据写入当前读取器
        /// </summary>
        /// <param name="writer">写入器</param>
        void WriteBuffer(IMemoryBufferWriter writer);
    }
}
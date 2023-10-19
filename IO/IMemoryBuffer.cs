using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

/****************************************************************
*  Copyright © (2023) www.eelf.cn All Rights Reserved.          *
*  Author : jacky                                               *
*  QQ : 7092734                                                 *
*  Email : jacky@eelf.cn                                        *
*  Site : www.eelf.cn                                           *
*  Create Time : 2023-10-19 14:40:41                            *
*  Version : v 1.0.0                                            *
*  CLR Version : 4.0.30319.42000                                *
*****************************************************************/
namespace XiaoFeng.IO
{
    /// <summary>
    /// 内存流接口
    /// </summary>
    public interface IMemoryBuffer
    {
        /// <summary>
        /// 最大可变长度
        /// </summary>
        ulong VariableByteIntegerMaxValue { get; set; }
        /// <summary>
        /// 读取位置
        /// </summary>
        long Position { get; set; }
        /// <summary>
        /// 包长度
        /// </summary>
        long Length { get; }
        /// <summary>
        /// 是否是结束
        /// </summary>
        bool EndOfStream { get; }
        /// <summary>
        /// 剩余未读长度
        /// </summary>
        long RemainingLength { get; }
        /// <summary>
        /// 将当前流的位置设置为指定值
        /// </summary>
        /// <param name="offset">流内的新位置。 它是相对于 origin 参数的位置，而且可正可负。</param>
        /// <param name="origin">类型 System.IO.SeekOrigin 的值，它用作查找引用点。</param>
        /// <returns>流内的新位置，通过将初始引用点和偏移量合并计算而得。</returns>
        long Seek(long offset, SeekOrigin origin);
        /// <summary>
        /// 移位到最开始
        /// </summary>
        void SeekFirst();
        /// <summary>
        /// 移位到结束
        /// </summary>
        void SeekLast();
        /// <summary>
        /// 向前移位
        /// </summary>
        /// <param name="offset">移动位置</param>
        /// <returns></returns>
        long Prev(ulong offset = 1);
        /// <summary>
        /// 向后移位
        /// </summary>
        /// <param name="offset">移动位置</param>
        /// <returns></returns>
        long Next(ulong offset = 1);
        /// <summary>
        /// 清空流
        /// </summary>
        void Clear();
        /// <summary>
        /// 创建此流的无符号字节的数组。
        /// </summary>
        byte[] GetBuffer();
        /// <summary>
        /// 将流内容写入字节数组，而与 Position 属性无关。
        /// </summary>
        /// <returns></returns>
        byte[] ToArray();
        /// <summary>
        /// 向当前内存流写入数据
        /// </summary>
        /// <param name="buffer">数据</param>
        /// <param name="offset">位置</param>
        /// <param name="count">长度</param>
        void WriteBuffer(byte[] buffer, int offset, int count);
        /// <summary>
        /// 向当前内存流写入数据
        /// </summary>
        /// <param name="buffer">数据</param>
        void WriteBuffer(byte[] buffer);
        /// <summary>
        /// 字节转数值
        /// </summary>
        /// <param name="values">字节组</param>
        /// <returns></returns>
        long BytesToValue(byte[] values);
        /// <summary>
        /// 数值转字节
        /// </summary>
        /// <param name="val">数值</param>
        /// <param name="length">字节数组长度</param>
        /// <returns></returns>
        byte[] ValueToBytes(long val, int length);
        /// <summary>
        /// 写字节
        /// </summary>
        /// <param name="byte">字节</param>
        void WriteByte(byte @byte);
        /// <summary>
        /// 获取可变字节长度占用字节数
        /// </summary>
        /// <param name="length">可变字节长度</param>
        /// <returns></returns>
        /// <exception cref="Exception">超过指定长度则抛出异常</exception>
        int GetVariableByteIntegerSize(ulong length);
        /// <summary>
        /// 获取可变字节长度字节数据
        /// </summary>
        /// <param name="length">值</param>
        /// <exception cref="Exception">异常</exception>
        byte[] EncodeVariableByteInteger(ulong length);
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
        /// 读取2个字节整型数字
        /// </summary>
        /// <returns></returns>
        ushort ReadTwoByteInteger();
        /// <summary>
        /// 读取4个字节整型数字
        /// </summary>
        /// <returns></returns>
        uint ReadFourByteInteger();
    }
}